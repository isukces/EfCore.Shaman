#region using

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Reflection;
using EfCore.Shaman.Reflection;
using Microsoft.EntityFrameworkCore;

#endregion

namespace EfCore.Shaman.ModelScanner
{
    public class ModelInfo
    {
        #region Constructors

        public ModelInfo(Type contextType, IList<IShamanService> services)
        {
            _contextType = contextType;
            _services = services;
            Prepare();
        }

        #endregion

        #region Static Methods

        public static bool NotNullFromPropertyType(Type type)
        {
            if (type == typeof(string)) return false;
            if (type.IsGenericType && (type.GetGenericTypeDefinition() == typeof(Nullable<>)))
                return false;
            if (type.IsEnum || type.IsValueType) return true;
            return false;
        }

        private static string GetTableName(Type entityType, string propertyName)
        {
            var a = entityType.GetCustomAttribute<TableAttribute>();
            return string.IsNullOrEmpty(a?.Name) ? propertyName : a.Name;
        }

        #endregion

        #region Instance Methods

        public DbSetInfo GetByTableName(string tableName)
        {
            DbSetInfo entity;
            _dbSets.TryGetValue(tableName, out entity);
            return entity;
        }


        private DbSetInfo CreateDbSetWrapper(Type entityType, string propertyName)
        {
            var dbSetInfoUpdateServices = _services?.OfType<IDbSetInfoUpdateService>().ToArray();

            var dbSetInfo = new DbSetInfo(entityType, GetTableName(entityType, propertyName));
            {
                if (dbSetInfoUpdateServices != null)
                    foreach (var i in dbSetInfoUpdateServices)
                        i.UpdateDbSetInfo(dbSetInfo, entityType);
            }
            var columnInfoUpdateServices = _services?.OfType<IColumnInfoUpdateService>().ToArray();
            foreach (var propertyInfo in entityType.GetProperties())
            {
                var columnInfo = new ColumnInfo(dbSetInfo.Properites.Count, propertyInfo.Name)
                {
                    NotNull = NotNullFromPropertyType(propertyInfo.PropertyType)
                };
                var readerWriter = new SimplePropertyReaderWriter(entityType, propertyInfo);
                columnInfo.ValueReader = readerWriter;
                columnInfo.ValueWriter = readerWriter;

                if (columnInfoUpdateServices != null)
                    foreach (var service in columnInfoUpdateServices)
                        service.UpdateColumnInfo(columnInfo, propertyInfo);
                dbSetInfo.Properites.Add(columnInfo);
            }
            return dbSetInfo;
        }


        private void Prepare()
        {
            foreach (var property in _contextType.GetProperties())
            {
                var propertyType = property.PropertyType;
                if (!propertyType.IsGenericType) continue;
                if (propertyType.GetGenericTypeDefinition() != typeof(DbSet<>)) continue;
                var entityType = propertyType.GetGenericArguments()[0];
                var entity = CreateDbSetWrapper(entityType, property.Name);
                _dbSets[entity.TableName] = entity;
            }
        }

        #endregion

        #region Properties

        public IEnumerable<DbSetInfo> DbSets => _dbSets.Values;

        #endregion

        #region Fields

        private readonly Type _contextType;
        private readonly IList<IShamanService> _services;


        private readonly Dictionary<string, DbSetInfo> _dbSets =
            new Dictionary<string, DbSetInfo>(StringComparer.OrdinalIgnoreCase);

        #endregion
    }
}