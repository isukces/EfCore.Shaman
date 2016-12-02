using System.ComponentModel.DataAnnotations;
using System.Reflection;
using EfCore.Shaman.ModelScanner;

namespace EfCore.Shaman.Services
{
    class KeyAttributeUpdater : IColumnInfoUpdateService
    {
        public void UpdateColumnInfo(ColumnInfo columnInfo, PropertyInfo propertyInfo)
        {
            var notMappedAttribute = propertyInfo.GetCustomAttribute<KeyAttribute>();
            columnInfo.IsInPrimaryKey = notMappedAttribute != null;
        }
    }
}