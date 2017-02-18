using System.ComponentModel.DataAnnotations;
using System.Reflection;
using EfCore.Shaman.ModelScanner;

namespace EfCore.Shaman.Services
{
    class RequiredAttributeUpdater : IColumnInfoUpdateService
    {
        public void UpdateColumnInfo(ColumnInfo columnInfo, PropertyInfo propertyInfo, IShamanLogger logger)
        {
            var attribute = propertyInfo.GetCustomAttribute<RequiredAttribute>();
            if (attribute == null) return;
            columnInfo.NotNull = true;
            logger.Log(typeof(RequiredAttributeUpdater), nameof(UpdateColumnInfo),
                $"Set NotNull=true for column '{columnInfo.ColumnName}'");
        }
    }
}
