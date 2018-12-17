using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection;

namespace EfCore.Shaman
{
    public class DefaultToColumnMappingService : IToColumnMappingService
    {
        private static bool InternalCheck(PropertyInfo propertyInfo)
        {
            if (propertyInfo.GetCustomAttribute<NotMappedAttribute>() != null)
                return false;
            if (propertyInfo.GetCustomAttribute<NavigationPropertyAttribute>() != null)
                return false;

            return true;
        }

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
    }

    public class CheckIgnoreEventArgs : EventArgs
    {
        public PropertyInfo Info     { get; set; }
        public bool         IsMapped { get; set; }
    }
}