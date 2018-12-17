using System;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
using EfCore.Shaman.ModelScanner;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EfCore.Shaman.Services
{
    public class MaxLengthAttributeUpdater : IColumnInfoUpdateService
    {
        public void UpdateColumnInfoInModelInfo(ColumnInfo columnInfo,
            IDbSetInfo dbSetInfo, IShamanLogger logger)
        {
            var attribute = columnInfo.ClrProperty?.GetCustomAttribute<MaxLengthAttribute>();
            if (attribute == null) return;
            columnInfo.MaxLength = attribute.Length;
            // todo log MaxLengthAttributeUpdater.UpdateColumnInfo
        }

        public void UpdateColumnInfoForMigrationFixer(ISimpleModelInfo modelInfo, IDbSetInfo dbSetInfo, ColumnInfo columnInfo,
            EntityTypeBuilder entityBuilder,
            IShamanLogger logger)
        {
        }
    }
}