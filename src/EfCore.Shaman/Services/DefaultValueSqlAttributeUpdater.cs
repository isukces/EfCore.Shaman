using System.Reflection;
using EfCore.Shaman.ModelScanner;
using System;

namespace EfCore.Shaman.Services
{
    internal class DefaultValueSqlAttributeUpdater : IColumnInfoUpdateService
    {
        #region Instance Methods

        public void UpdateColumnInfo(ColumnInfo columnInfo, PropertyInfo propertyInfo)
        {
            var attribute = propertyInfo.GetCustomAttribute<DefaultValueSqlAttribute>();
            if (attribute == null ) return;
            columnInfo.DefaultValueSql = attribute.ValueSql;
        }

        #endregion
    }
}