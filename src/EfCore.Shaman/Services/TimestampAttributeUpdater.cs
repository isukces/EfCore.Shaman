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
        public void UpdateColumnInfoInModelInfo(ColumnInfo columnInfo, PropertyInfo propertyInfo, IShamanLogger logger)
        {
            if (propertyInfo.GetCustomAttribute<TimestampAttribute>() == null) return;
            columnInfo.IsTimestamp = true;
            logger.Log(typeof(TimestampAttributeUpdater), nameof(UpdateColumnInfoForMigrationFixer),
                $"{columnInfo.ColumnName}.IsTimestamp=true");
            columnInfo.IsDatabaseGenerated = true;
            logger.Log(typeof(TimestampAttributeUpdater), nameof(UpdateColumnInfoForMigrationFixer),
                $"{columnInfo.ColumnName}.IsDatabaseGenerated=true");
        }

        public void UpdateColumnInfoForMigrationFixer(ISimpleModelInfo modelInfo, IDbSetInfo dbSetInfo, ColumnInfo columnInfo,
            EntityTypeBuilder entityBuilder,
            IShamanLogger logger)
        {
        }
    }
}