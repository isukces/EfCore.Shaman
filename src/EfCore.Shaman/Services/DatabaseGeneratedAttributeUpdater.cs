#region using

using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection;
using EfCore.Shaman.ModelScanner;

#endregion

namespace EfCore.Shaman.Services
{
    internal class DatabaseGeneratedAttributeUpdater : IColumnInfoUpdateService
    {
        #region Instance Methods

        public void UpdateColumnInfo(ColumnInfo columnInfo, PropertyInfo propertyInfo)
        {
            var at = propertyInfo.GetCustomAttribute<DatabaseGeneratedAttribute>();
            if (at == null)
                return;
            columnInfo.IsDatabaseGenerated = true;
            columnInfo.IsIdentity = at.DatabaseGeneratedOption == DatabaseGeneratedOption.Identity;
        }

        #endregion
    }
}