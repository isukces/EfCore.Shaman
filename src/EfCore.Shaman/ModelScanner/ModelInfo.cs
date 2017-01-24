#region using

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Reflection;
using EfCore.Shaman.Reflection;
using EfCore.Shaman.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

#endregion

namespace EfCore.Shaman.ModelScanner
{
    public class ModelInfo
    {
        #region Constructors

        public ModelInfo(Type dbContextType, IList<IShamanService> services = null)
        {
            _dbContextType = dbContextType;
            _services = services ?? ShamanOptions.CreateShamanOptions(dbContextType).Services;
            Prepare();
        }

        #endregion

        #region Static Methods

        public static bool NotNullFromPropertyType(Type type)
        {
            if (type == typeof(string)) return false;
            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
                return false;
            if (type.IsEnum || type.IsValueType) return true;
            return false;
        }

        private static string GetTableName(Type entityType, string propertyName, IReadOnlyDictionary<Type, string> tableNames)
        {
            if (tableNames != null)
            {
                string name;
                if (tableNames.TryGetValue(entityType, out name))
                    return name;
            }
            var a = entityType.GetCustomAttribute<TableAttribute>();
            return string.IsNullOrEmpty(a?.Name) ? propertyName : a.Name;
        }

        #endregion

        #region Instance Methods

        public DbSetInfo DbSet<T>()
            => _dbSets.Values.SingleOrDefault(a => a.EntityType == typeof(T));

        public DbSetInfo GetByTableName(string tableName)
        {
            DbSetInfo entity;
            _dbSets.TryGetValue(tableName, out entity);
            return entity;
        }


        private DbSetInfo CreateDbSetWrapper(Type entityType, string propertyName, IReadOnlyDictionary<Type, string> tableNames)
        {
            var dbSetInfoUpdateServices = _services?.OfType<IDbSetInfoUpdateService>().ToArray();
            var dbSetInfo = new DbSetInfo(entityType, GetTableName(entityType, propertyName, tableNames), DefaultSchema);
            {
                if (dbSetInfoUpdateServices != null)
                    foreach (var i in dbSetInfoUpdateServices)
                        i.UpdateDbSetInfo(dbSetInfo, entityType, _dbContextType);
            }
            var columnInfoUpdateServices = _services?.OfType<IColumnInfoUpdateService>().ToArray();
            var useDirectSaverForType = entityType.GetCustomAttribute<NoDirectSaverAttribute>() == null;
            foreach (var propertyInfo in entityType.GetProperties())
            {
                var columnInfo = new ColumnInfo(dbSetInfo.Properites.Count, propertyInfo.Name)
                {
                    NotNull = NotNullFromPropertyType(propertyInfo.PropertyType)
                };
                if (useDirectSaverForType)
                {
                    var readerWriter = new SimplePropertyReaderWriter(entityType, propertyInfo);
                    columnInfo.ValueReader = readerWriter;
                    columnInfo.ValueWriter = readerWriter;
                }
                if (columnInfoUpdateServices != null)
                    foreach (var service in columnInfoUpdateServices)
                        service.UpdateColumnInfo(columnInfo, propertyInfo);
                dbSetInfo.Properites.Add(columnInfo);
            }
            return dbSetInfo;
        }

        static IReadOnlyDictionary<Type, string> GetTableNamesFromModel(IModel model)
        {
            var result = new Dictionary<Type, string>();
            foreach (var entityType in model.GetEntityTypes())
                result[entityType.ClrType] = entityType.Relational().TableName;
            return result;
        }

        private void Prepare()
        {
            // todo: bad design - make service

            var instance = TryCreateInstance(_dbContextType);
            IReadOnlyDictionary<Type, string> tableNames = null;
            if (instance != null)
            {
                instance.CreationMode = DbContextCreationMode.WithoutModelFixing;
                var model = ((DbContext)instance).Model;
                tableNames = GetTableNamesFromModel(model);
            }
            DefaultSchema = DefaultSchemaUpdater.GetDefaultSchema(_dbContextType);

            foreach (var property in _dbContextType.GetProperties())
            {
                var propertyType = property.PropertyType;
                if (!propertyType.IsGenericType) continue;
                if (propertyType.GetGenericTypeDefinition() != typeof(DbSet<>)) continue;
                var entityType = propertyType.GetGenericArguments()[0];
                var entity = CreateDbSetWrapper(entityType, property.Name, tableNames);
                _dbSets[entity.TableName] = entity;
            }
        }

        private IShamanFriendlyDbContext TryCreateInstance(Type contextType)
        {
            if (contextType == null) throw new ArgumentNullException(nameof(contextType));
            var implementsMyInterface = contextType.GetTypeInfo().ImplementedInterfaces.Contains(typeof(IShamanFriendlyDbContext));
            if (!implementsMyInterface)
                return null; // can't create instance because it can affect stackoverflowexception due to calling FixOnModelCreating
            return InstanceCreator.CreateInstance(contextType) as IShamanFriendlyDbContext;
        }

        #endregion

        #region Properties

        public string DefaultSchema { get; set; }

        public IEnumerable<DbSetInfo> DbSets => _dbSets.Values;

        #endregion

        #region Fields

        private readonly Type _dbContextType;
        private readonly IList<IShamanService> _services;


        private readonly Dictionary<string, DbSetInfo> _dbSets =
            new Dictionary<string, DbSetInfo>(StringComparer.OrdinalIgnoreCase);

        #endregion
    }
}