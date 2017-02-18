using System.Reflection;
using EfCore.Shaman.ModelScanner;

namespace EfCore.Shaman.Services
{
    internal class DecimalTypeAttributeUpdater : IColumnInfoUpdateService
    {
        #region Instance Methods

        public void UpdateColumnInfo(ColumnInfo columnInfo, PropertyInfo propertyInfo, IShamanLogger logger)
        {
            var indexAttribute = propertyInfo.GetCustomAttribute<DecimalTypeAttribute>();
            if (indexAttribute == null) return;
            logger.Log(typeof(DecimalTypeAttributeUpdater), nameof(UpdateColumnInfo),
                $"Set MaxLength={indexAttribute.Length}, DecimalPlaces={indexAttribute.DecimalPlaces} for column '{columnInfo.ColumnName}'");
            columnInfo.MaxLength = indexAttribute.Length;
            columnInfo.DecimalPlaces = indexAttribute.DecimalPlaces;
        }

        #endregion
    }
}