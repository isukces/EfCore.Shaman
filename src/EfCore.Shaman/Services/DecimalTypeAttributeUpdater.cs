using System.Linq;
using System.Reflection;
using EfCore.Shaman.ModelScanner;

namespace EfCore.Shaman.Services
{
    internal class DecimalTypeAttributeUpdater : IColumnInfoUpdateService
    {
        #region Instance Methods

        public void UpdateColumnInfo(ColumnInfo columnInfo, PropertyInfo propertyInfo)
        {
            var indexAttribute = propertyInfo.GetCustomAttribute<DecimalTypeAttribute>();
            if (indexAttribute == null ) return;
            columnInfo.MaxLength = indexAttribute.Length;
            columnInfo.DecimalPlaces = indexAttribute.DecimalPlaces;
        }

        #endregion
    }
}