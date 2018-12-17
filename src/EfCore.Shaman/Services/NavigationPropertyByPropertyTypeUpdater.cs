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
            CollectionsGenericTypes = new HashSet<Type>
            {
                typeof(IList<>), typeof(List<>), typeof(ICollection<>), typeof(IEnumerable<>)
            };
        }

        public HashSet<Type> CollectionsGenericTypes { get;  }

        public void UpdateColumnInfoOnModelCreating(IDbSetInfo dbSetInfo,
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
            if (propertyInfo.PropertyType == typeof(string) || propertyInfo.PropertyType.IsEnum ||
                propertyInfo.PropertyType.IsPrimitive)
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
            if (!CollectionsGenericTypes.Contains(gtt)) return false;
            var arg = type.GetGenericArguments()[0];
            return IsEntityType(arg);
        }
        

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
