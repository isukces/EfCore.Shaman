using System.ComponentModel.DataAnnotations;
using System.Reflection;
using EfCore.Shaman.ModelScanner;

namespace EfCore.Shaman.Services
{
    class MaxLengthAttributeUpdater : IColumnInfoUpdateService
    {
        public void UpdateColumnInfo(ColumnInfo columnInfo, PropertyInfo propertyInfo, IShamanLogger logger)
        {
            var attribute = propertyInfo.GetCustomAttribute<MaxLengthAttribute>();
            if (attribute == null) return;
            columnInfo.MaxLength = attribute.Length;
            // todo log MaxLengthAttributeUpdater.UpdateColumnInfo
        }
    }
}