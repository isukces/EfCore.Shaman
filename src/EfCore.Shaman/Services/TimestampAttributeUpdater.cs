using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection;
using EfCore.Shaman.ModelScanner;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EfCore.Shaman.Services
{
    internal class TimestampAttributeUpdater : IColumnInfoUpdateService
    {
        public void ModelInfoUpdateColumnInfo(ColumnInfo columnInfo, PropertyInfo propertyInfo, IShamanLogger logger)
        {
            if (propertyInfo.GetCustomAttribute<TimestampAttribute>() == null) return;
            columnInfo.IsTimestamp = true;
            logger.Log(typeof(TimestampAttributeUpdater), nameof(ModelFixerUpdateColumnInfo),
                $"{columnInfo.ColumnName}.IsTimestamp=true");
            columnInfo.IsDatabaseGenerated = true;
            logger.Log(typeof(TimestampAttributeUpdater), nameof(ModelFixerUpdateColumnInfo),
                $"{columnInfo.ColumnName}.IsDatabaseGenerated=true");
        }

        public void ModelFixerUpdateColumnInfo(ColumnInfo columnInfo, EntityTypeBuilder entityBuilder, Type entityType,
            IShamanLogger logger)
        {
        }
    }
}