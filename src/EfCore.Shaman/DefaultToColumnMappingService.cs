using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Reflection;

namespace EfCore.Shaman
{
    public class DefaultToColumnMappingService : IToColumnMappingService
    {
        private Type _dbContextType;
        private ImmutableHashSet<Type> _entityTypes;

        private bool InternalCheck(PropertyInfo propertyInfo)
        {
            if (propertyInfo.GetCustomAttribute<NotMappedAttribute>() != null)
                return false;
            if (propertyInfo.GetCustomAttribute<NavigationPropertyAttribute>() != null)
                return false;
            var type = propertyInfo.GetType();
            var gtt = type.IsGenericType ? type.GetGenericTypeDefinition() : null;
            if (gtt == typeof(Nullable<>))
                type = type.GenericTypeArguments[0];
            if (type.IsPrimitive)
                return true;
            if (type == typeof(Guid) 
                || type == typeof(string)  
                || type == typeof(byte[])  
                || type == typeof(decimal) 
                || type == typeof(DateTime) 
                || type == typeof(DateTimeOffset))
                return true;
            if (type.IsEnum)
                return true;
            if (_entityTypes!=null)
            if (_entityTypes.Contains(type))
                return false;
            if (DoNotMapCollections)
                if (gtt == typeof(IList<>) || gtt == typeof(List<>) || gtt == typeof(ICollection<>) ||
                    gtt == typeof(IEnumerable<>))
                    return false;
            return true;             
        }

        public bool DoNotMapCollections { get; set; } = true;

        public bool IsMappedToTableColumn(PropertyInfo propertyInfo)
        {
            var isMapped = InternalCheck(propertyInfo);
            var handler  = CheckIsMapped;
            if (handler == null)
                return isMapped;
            var args = new CheckIgnoreEventArgs
            {
                Info     = propertyInfo,
                IsMapped = isMapped
            };
            handler.Invoke(this, args);
            return args.IsMapped;
        }

        public event EventHandler<CheckIgnoreEventArgs> CheckIsMapped;

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
                    _entityTypes = ModelScanner.ModelInfo.GetEntityTypes(value)
                        .Select(a => a.EntityType)
                        .ToImmutableHashSet();
            }
        }
    }

    public class CheckIgnoreEventArgs : EventArgs
    {
        public PropertyInfo Info     { get; set; }
        public bool         IsMapped { get; set; }
    }
}