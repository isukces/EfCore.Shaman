#region using

using System.Reflection;
using EfCore.Shaman.ModelScanner;

#endregion

namespace EfCore.Shaman.Services
{
    internal class DefaultValueSqlAttributeUpdater : IColumnInfoUpdateService
    {
        #region Instance Methods

        public void UpdateColumnInfo(ColumnInfo columnInfo, PropertyInfo propertyInfo, IShamanLogger logger)
        {
            var attribute = propertyInfo.GetCustomAttribute<DefaultValueSqlAttribute>();
            if (!string.IsNullOrEmpty(attribute?.DefaultValueSql))
                columnInfo.DefaultValue = ValueInfo.FromSqlValue(attribute.DefaultValueSql);
            // todo: log
        }

        #endregion
    }
}