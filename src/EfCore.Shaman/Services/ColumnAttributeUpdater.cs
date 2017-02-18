using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection;
using EfCore.Shaman.ModelScanner;

namespace EfCore.Shaman.Services
{
    class ColumnAttributeUpdater : IColumnInfoUpdateService
    {
        public void UpdateColumnInfo(ColumnInfo columnInfo, PropertyInfo propertyInfo, IShamanLogger logger)
        {
            var attribute = propertyInfo.GetCustomAttribute<ColumnAttribute>();
            if (attribute == null) return;
            if (!string.IsNullOrEmpty(attribute.Name))
            {
                columnInfo.ColumnName = attribute.Name;
                logger.Log(typeof(ColumnAttributeUpdater), "UpdateColumnInfo",
                    $"Set ColumnName='{columnInfo.ColumnName}'");
            }
            if (attribute.Order >= 0)
            {
                columnInfo.ForceFieldOrder = attribute.Order;
                logger.Log(typeof(ColumnAttributeUpdater), "UpdateColumnInfo",
                    $"Set ForceFieldOrder={columnInfo.ForceFieldOrder}");
            }
        }
    }
}
