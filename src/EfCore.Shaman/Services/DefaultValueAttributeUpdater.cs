using System.Reflection;
using EfCore.Shaman.ModelScanner;
using System.ComponentModel;
using System;

namespace EfCore.Shaman.Services
{
    internal class DefaultValueAttributeUpdater : IColumnInfoUpdateService
    {
        #region Instance Methods

        public void UpdateColumnInfo(ColumnInfo columnInfo, PropertyInfo propertyInfo)
        {
            var attribute = propertyInfo.GetCustomAttribute<DefaultValueAttribute>();
            if (attribute == null) return;
            columnInfo.DefaultValue = attribute.Value;
        }

        #endregion
    }
}