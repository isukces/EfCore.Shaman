#region using

using System.ComponentModel;
using System.Reflection;
using EfCore.Shaman.ModelScanner;

#endregion

namespace EfCore.Shaman.Services
{
    internal class DefaultValueAttributeUpdater : IColumnInfoUpdateService
    {
        #region Instance Methods

        public void UpdateColumnInfo(ColumnInfo columnInfo, PropertyInfo propertyInfo)
        {
            var attribute = propertyInfo.GetCustomAttribute<DefaultValueAttribute>();
            if (attribute != null)
                columnInfo.DefaultValue = ValueInfo.FromClrValue(attribute.Value);
        }

        #endregion
    }
}