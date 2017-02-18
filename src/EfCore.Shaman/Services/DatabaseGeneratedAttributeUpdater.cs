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

        public void UpdateColumnInfo(ColumnInfo columnInfo, PropertyInfo propertyInfo, IShamanLogger logger)
        {
            var at = propertyInfo.GetCustomAttribute<DatabaseGeneratedAttribute>();
            if (at == null)
                return;
            logger.Log(
                typeof(DatabaseGeneratedAttributeUpdater),
                nameof(UpdateColumnInfo),
                $"Set IsDatabaseGenerated=true and DatabaseGeneratedOption.Identity for column {columnInfo.ColumnName}");
            columnInfo.IsDatabaseGenerated = true;
            columnInfo.IsIdentity = at.DatabaseGeneratedOption == DatabaseGeneratedOption.Identity;
        }

        #endregion
    }
}