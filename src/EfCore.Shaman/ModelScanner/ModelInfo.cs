using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Reflection;
using Microsoft.EntityFrameworkCore;

namespace EfCore.Shaman.ModelScanner
{
    public class ModelInfo
    {
        #region Constructors

        public ModelInfo(Type contextType)
        {
            _contextType = contextType;
            Prepare();
        }

        public IEnumerable<DbSetInfo> DbSets => _dbSets.Values;

        #endregion

        #region Instance Methods

        public DbSetInfo GetByTableName(string tableName)
        {
            DbSetInfo entity;
            _dbSets.TryGetValue(tableName, out entity);
            return entity;
        }


        private static DbSetInfo CreateDbSetWrapper(Type entityType, string propertyName)
        {
            var dbSetWrapper = new DbSetInfo(entityType, propertyName);
            {
                var ta = entityType.GetCustomAttribute<TableAttribute>();
                if (ta != null)
                {
                    dbSetWrapper.TableName = ta.Name;
                    dbSetWrapper.Schema = ta.Schema;
                }
            }
            foreach (var propertyInfo in entityType.GetProperties())
            {
                var propertyWrapper = new ColumnInfo(dbSetWrapper.Properites.Count, propertyInfo.Name);
                var columnAttribute = propertyInfo.GetCustomAttribute<ColumnAttribute>();
                if (columnAttribute != null)
                {
                    if (!string.IsNullOrEmpty(columnAttribute.Name))
                        propertyWrapper.ColumnName = columnAttribute.Name;
                    propertyWrapper.ForceFieldOrder = columnAttribute.Order;
                }
                var notMappedAttribute = propertyInfo.GetCustomAttribute<NotMappedAttribute>();
                propertyWrapper.IsNotMapped = notMappedAttribute != null;

                var indexAttributes = propertyInfo.GetCustomAttributes<AbstractIndexAttribute>()?.ToArray();
                if (indexAttributes != null && indexAttributes.Any())
                {
                    foreach (var indexAttribute in indexAttributes)
                    {
                        var indexMember = new ColumnIndexInfo
                        {
                            IndexName = indexAttribute.Name?.Trim(),
                            Order = indexAttribute.Order,
                            IsDescending = indexAttribute.IsDescending,
                            IsUnique = indexAttribute is UniqueIndexAttribute
                        };
                        propertyWrapper.ColumnIndexes.Add(indexMember);
                    }
                }

                dbSetWrapper.Properites.Add(propertyWrapper);
            }
            return dbSetWrapper;
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

        #region Fields

        private readonly Type _contextType;


        private readonly Dictionary<string, DbSetInfo> _dbSets =
            new Dictionary<string, DbSetInfo>(StringComparer.OrdinalIgnoreCase);

        #endregion
    }
}