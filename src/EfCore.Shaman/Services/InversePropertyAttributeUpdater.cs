#region using

using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection;
using EfCore.Shaman.ModelScanner;

#endregion

namespace EfCore.Shaman.Services
{
    internal class InversePropertyAttributeUpdater : IColumnInfoUpdateService
    {
        #region Instance Methods

        public void UpdateColumnInfo(ColumnInfo columnInfo, PropertyInfo propertyInfo)
        {
            if (propertyInfo.GetCustomAttribute<InversePropertyAttribute>() != null)
                columnInfo.IsNavigationProperty = true;
        }

        #endregion
    }
}