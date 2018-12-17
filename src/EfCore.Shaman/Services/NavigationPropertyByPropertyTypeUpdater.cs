using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection;
using EfCore.Shaman.ModelScanner;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EfCore.Shaman.Services
{
    public class NavigationPropertyByPropertyTypeUpdater : IColumnInfoUpdateService
    {
        public NavigationPropertyByPropertyTypeUpdater(Type dbContextType)
        {
            DbContextType = dbContextType;
        }

        private static bool CheckGenerticTypeIsCollection(Type gtt)
        {
            return gtt == typeof(IList<>)
                   || gtt == typeof(List<>)
                   || gtt == typeof(ICollection<>)
                   || gtt == typeof(IEnumerable<>);
        }

        public void UpdateColumnInfoForMigrationFixer(ISimpleModelInfo modelInfo, IDbSetInfo dbSetInfo,
            ColumnInfo columnInfo,
            EntityTypeBuilder entityBuilder, IShamanLogger logger)
        {
        }

        public void UpdateColumnInfoInModelInfo(ColumnInfo columnInfo, IDbSetInfo dbSetInfo,
            IShamanLogger logger)
        {
            if (columnInfo.IsNotMapped || columnInfo.IsNavigationProperty)
                return;
            var propertyInfo = columnInfo.ClrProperty;
            if (propertyInfo == null)
                return;
            var attribute = propertyInfo.GetCustomAttribute<NavigationPropertyAttribute>();
            if (attribute != null)
                return;
            if (IsEntityType(propertyInfo.PropertyType))
                columnInfo.IsNavigationProperty = true;
            else if (IsListOfEntityType(propertyInfo.PropertyType))
                columnInfo.IsNavigationProperty = true;
        }
        
        private bool IsEntityType(Type propertyInfoPropertyType)
        {
            return _entityTypes != null && _entityTypes.Contains(propertyInfoPropertyType);
        }

        private bool IsListOfEntityType(Type type)
        {
            if (!type.IsGenericType) return false;
            var gtt = type.GetGenericTypeDefinition();
            if (DoNotMapCollections) return false;
            if (!CheckGenerticTypeIsCollection(gtt)) return false;
            var arg = type.GetGenericArguments()[0];
            return IsEntityType(arg);
        }

        public bool DoNotMapCollections { get; set; } = true;

        public Type DbContextType
        {
            get => _dbContextType;
            set
            {
                if (_dbContextType == value)
                    return;
                _dbContextType = value;
                if (value == null)
                    _entityTypes = null;
                else
                    _entityTypes = ModelInfo.GetEntityTypes(value)
                        .Select(a => a.EntityType)
                        .ToImmutableHashSet();
            }
        }

        private Type _dbContextType;
        private ImmutableHashSet<Type> _entityTypes;
    }
}