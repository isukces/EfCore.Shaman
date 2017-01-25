using System.ComponentModel.DataAnnotations;
using System.Reflection;
using EfCore.Shaman.ModelScanner;

namespace EfCore.Shaman.Services
{
    class RequiredAttributeUpdater: IColumnInfoUpdateService
    {
        public void UpdateColumnInfo(ColumnInfo columnInfo, PropertyInfo propertyInfo)
        {
            var attribute = propertyInfo.GetCustomAttribute<RequiredAttribute>();
            if (attribute == null) return;
            columnInfo.NotNull = true;
        }
    }
}
