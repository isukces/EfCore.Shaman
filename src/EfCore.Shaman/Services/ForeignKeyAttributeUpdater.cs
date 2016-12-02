using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection;
using EfCore.Shaman.ModelScanner;

namespace EfCore.Shaman.Services
{
    internal class ForeignKeyAttributeUpdater : IColumnInfoUpdateService
    {
        #region Instance Methods

        public void UpdateColumnInfo(ColumnInfo columnInfo, PropertyInfo propertyInfo)
        {
            if (propertyInfo.GetCustomAttribute<ForeignKeyAttribute>() != null)
                columnInfo.IsNavigationProperty = true;
        }

        #endregion
    }
}