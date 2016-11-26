using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection;
using EfCore.Shaman.ModelScanner;

namespace EfCore.Shaman.Services
{
    class NotMappedAttributeUpdater : IColumnInfoUpdateService
    {
        public void UpdateColumnInfo(ColumnInfo columnInfo, PropertyInfo propertyInfo)
        {
            var notMappedAttribute = propertyInfo.GetCustomAttribute<NotMappedAttribute>();
            columnInfo.IsNotMapped = notMappedAttribute != null;
        }
    }
}