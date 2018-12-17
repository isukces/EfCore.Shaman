using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection;
using EfCore.Shaman.ModelScanner;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EfCore.Shaman.Services
{
    public  class TimestampAttributeUpdater : IColumnInfoUpdateService
    {
        public void UpdateColumnInfoInModelInfo(ColumnInfo columnInfo,
            IDbSetInfo dbSetInfo, IShamanLogger logger)
        {
            if (columnInfo.ClrProperty?.GetCustomAttribute<TimestampAttribute>() == null) return;
            var logPrefix = $"Set {dbSetInfo.TableName}.{columnInfo.ColumnName}";
            const string logSource =
                nameof(TimestampAttributeUpdater) + "." + nameof(UpdateColumnInfoForMigrationFixer);

            columnInfo.IsTimestamp = true;
            logger.Log(logSource, $"{logPrefix}.IsTimestamp=true");
            columnInfo.IsDatabaseGenerated = true;
            logger.Log(logSource, $"{logPrefix}.IsDatabaseGenerated=true");
        }

        public void UpdateColumnInfoForMigrationFixer(ISimpleModelInfo modelInfo, IDbSetInfo dbSetInfo, ColumnInfo columnInfo,
            EntityTypeBuilder entityBuilder,
            IShamanLogger logger)
        {
        }
    }
}