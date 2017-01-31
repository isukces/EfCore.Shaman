using System.Reflection;
using EfCore.Shaman.ModelScanner;

namespace EfCore.Shaman.Services
{
    class DefaultValueAttributeUpdater : IColumnInfoUpdateService
    {
        public void UpdateColumnInfo(ColumnInfo columnInfo, PropertyInfo propertyInfo)
        {
            var attribute = propertyInfo.GetCustomAttribute<DefaultValueAttribute>();
            if (attribute == null) return;
            columnInfo.DefaultValue = new ValueInfo(attribute.DefaultValue);
        }
    }
   
}