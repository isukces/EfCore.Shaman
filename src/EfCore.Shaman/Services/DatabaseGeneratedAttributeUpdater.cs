#region using

using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection;
using EfCore.Shaman.ModelScanner;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

#endregion

namespace EfCore.Shaman.Services
{
    public  class DatabaseGeneratedAttributeUpdater : IColumnInfoUpdateService
    {
        public void UpdateColumnInfoInModelInfo(ColumnInfo columnInfo,
            IDbSetInfo dbSetInfo, IShamanLogger logger)
        {
            var at = columnInfo.ClrProperty?.GetCustomAttribute<DatabaseGeneratedAttribute>();
            if (at == null)
                return;
            logger.Log(
                typeof(DatabaseGeneratedAttributeUpdater),
                nameof(UpdateColumnInfoForMigrationFixer),
                $"Set IsDatabaseGenerated=true and DatabaseGeneratedOption.Identity for column {columnInfo.ColumnName}");
            columnInfo.IsDatabaseGenerated = true;
            columnInfo.IsIdentity = at.DatabaseGeneratedOption == DatabaseGeneratedOption.Identity;
        }
        public void UpdateColumnInfoForMigrationFixer(ISimpleModelInfo modelInfo, IDbSetInfo dbSetInfo, ColumnInfo columnInfo,
            EntityTypeBuilder entityBuilder,
            IShamanLogger logger)
        {
        }

    }
}