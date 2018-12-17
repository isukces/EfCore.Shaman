using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Reflection;
using EfCore.Shaman.Reflection;
using EfCore.Shaman.Services;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;

namespace EfCore.Shaman.ModelScanner
{
    public class ModelInfo : ISimpleModelInfo, IUpdatableSimpleModelInfo
    {
        public ModelInfo(Type dbContextType, ShamanOptions options = null)
        {
            _dbContextType    = dbContextType;
            options           = options ?? ShamanOptions.CreateShamanOptions(dbContextType);
            UsedShamanOptions = options;
            _logger           = options.Logger ?? EmptyShamanLogger.Instance;
            Prepare();
        }

        public static IEnumerable<A123> GetEntityTypes(Type dbContextType)
        {
            foreach (var property in dbContextType.GetProperties())
            {
                var propertyType = property.PropertyType;
                if (!propertyType.GetTypeInfo().IsGenericType) continue;
                if (propertyType.GetGenericTypeDefinition() != typeof(DbSet<>)) continue;
                var entityType = propertyType.GetGenericArguments()[0];
                yield return new A123(property, entityType);
            }
        }

        public static ModelInfo Make<T>(ShamanOptions options = null)
        {
            return new ModelInfo(typeof(T), options);
        }

        public static bool NotNullFromPropertyType(Type type)
        {
            if (type == typeof(string)) return false;
            var typeInfo = type.GetTypeInfo();
            if (typeInfo.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
                return false;
            if (typeInfo.IsEnum || typeInfo.IsValueType) return true;
            return false;
        }

        private static string GetTableName(Type entityType, string propertyName,
            IReadOnlyDictionary<Type, string> tableNames)
        {
            if (tableNames != null)
            {
                string name;
                if (tableNames.TryGetValue(entityType, out name))
                    return name;
            }

            var a = entityType.GetTypeInfo().GetCustomAttribute<TableAttribute>();
            return string.IsNullOrEmpty(a?.Name) ? propertyName : a.Name;
        }

        private static IReadOnlyDictionary<Type, string> GetTableNamesFromModel(EfModelWrapper model,
            IShamanLogger logger)
        {
            if (model == null) return null;
            var result = new Dictionary<Type, string>();
            foreach (var entityType in model.EntityTypes)
            {
                result[entityType.ClrType] = entityType.TableName;
                logger.Log(typeof(ModelInfo), nameof(GetTableNamesFromModel),
                    $"Table name for {entityType.ClrType.Name}: {entityType.TableName}");
            }

            return result;
        }

        public DbSetInfo DbSet<T>()
            => _dbSets.Values.SingleOrDefault(a => a.EntityType == typeof(T));

        public DbSetInfo GetByTableName(FullTableName tableName)
        {
            _dbSets.TryGetValue(tableName, out var entity);
            return entity;
        }

        private DbSetInfo CreateDbSetWrapper(Type entityType, string propertyName,
            IReadOnlyDictionary<Type, string> tableNames)
        {
            var dbSetInfoUpdateServices = UsedShamanOptions.Services?.OfType<IDbSetInfoUpdateService>().ToArray();
            var dbSetInfo = new DbSetInfo(entityType, GetTableName(entityType, propertyName, tableNames),
                DefaultSchema);
            {
                if (dbSetInfoUpdateServices != null)
                    foreach (var i in dbSetInfoUpdateServices)
                        i.UpdateDbSetInfo(dbSetInfo, entityType, _dbContextType, _logger);
            }
            var columnInfoUpdateServices = UsedShamanOptions.Services?.OfType<IColumnInfoUpdateService>().ToArray();
            var useDirectSaverForType =
                entityType.GetTypeInfo().GetCustomAttribute<NoDirectSaverAttribute>() == null;
            foreach (var propertyInfo in entityType.GetProperties(BindingFlags.Instance | BindingFlags.Public))
            {
                var columnInfo = new ColumnInfo(dbSetInfo.Properites.Count, propertyInfo.Name)
                {
                    NotNull = NotNullFromPropertyType(propertyInfo.PropertyType)
                };
                if (useDirectSaverForType)
                {
                    var readerWriter = new SimplePropertyReaderWriter(entityType, propertyInfo, _logger);
                    columnInfo.ValueReader = readerWriter;
                    columnInfo.ValueWriter = readerWriter;
                }

                if (columnInfoUpdateServices != null)
                    foreach (var service in columnInfoUpdateServices)
                        service.UpdateColumnInfoInModelInfo(columnInfo, propertyInfo, dbSetInfo, _logger);
                dbSetInfo.Properites.Add(columnInfo);
            }

            return dbSetInfo;
        }

        private void Log(string methodName, string message)
        {
            _logger.Log(typeof(ModelInfo), methodName, message);
        }

        private void Prepare()
        {
            // todo: bad design - make service
            var model = ModelsCachedContainer.GetRawModel(_dbContextType, _logger);
            UsedDbContextModel = model != null;
            Log(nameof(Prepare),
                UsedDbContextModel
                    ? "Use dbContext model"
                    : "Don't use dbContext model");
            var tableNames = GetTableNamesFromModel(model, _logger);
            DefaultSchema        = DefaultSchemaUpdater.GetDefaultSchema(_dbContextType, model, _logger);
            DefaultIsUnicodeText = DefaultSchemaUpdater.GetDefaultIsUnicodeText(_dbContextType, _logger);
            foreach (var i in UsedShamanOptions.Services.OfType<IModelPrepareService>())
                i.UpdateModel(this, _dbContextType, _logger);
            foreach (var property in _dbContextType.GetProperties())
            {
                var propertyType = property.PropertyType;
                if (!propertyType.GetTypeInfo().IsGenericType) continue;
                if (propertyType.GetGenericTypeDefinition() != typeof(DbSet<>)) continue;
                var entityType = propertyType.GetGenericArguments()[0];
                var entity     = CreateDbSetWrapper(entityType, property.Name, tableNames);
                var key        = new FullTableName(entity.TableName, entity.Schema);
                _dbSets[key] = entity;
            }

            foreach (var aa in GetEntityTypes(_dbContextType))
            {
                var entity = CreateDbSetWrapper(aa.EntityType, aa.Property.Name, tableNames);
                var key    = new FullTableName(entity.TableName, entity.Schema);
                _dbSets[key] = entity;
            }
        }

        [NotNull]
        public ShamanOptions UsedShamanOptions { get; }

        public string DefaultSchema { get; set; }

        public bool DefaultIsUnicodeText { get; set; } = true;

        public IEnumerable<DbSetInfo> DbSets => _dbSets.Values;

        // public IList<IShamanService> UsedServices { get; private set; }

        /// <summary>
        ///     IModel from DbContext has been used in modelinfo building
        /// </summary>
        public bool UsedDbContextModel { get; private set; }

        public IDictionary<string, object> Annotations { get; } = new Dictionary<string, object>();

        private readonly Type _dbContextType;


        private readonly Dictionary<FullTableName, DbSetInfo> _dbSets =
            new Dictionary<FullTableName, DbSetInfo>();

        private readonly IShamanLogger _logger;

        public struct A123
        {
            public A123(PropertyInfo property, Type entityType)
            {
                Property   = property;
                EntityType = entityType;
            }

            public PropertyInfo Property   { get; }
            public Type         EntityType { get; }
        }
    }
}