using System;
using System.Reflection;
using EfCore.Shaman.ModelScanner;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EfCore.Shaman
{
    public class UnicodeColumnInfoUpdateService : IColumnInfoUpdateService
    {
        public void UpdateColumnInfoInModelInfo(ColumnInfo columnInfo,
            IDbSetInfo dbSetInfo, IShamanLogger logger)
        {
        }

        public void UpdateColumnInfoOnModelCreating(IDbSetInfo dbSetInfo, ColumnInfo columnInfo,
            EntityTypeBuilder entityBuilder,
            IShamanLogger logger)
        {
            const string name = nameof(UnicodeColumnInfoUpdateService) + "." + nameof(UpdateColumnInfoOnModelCreating);
            if (columnInfo.IsUnicode == null)
                return;
            var action = $"IsUnicode({columnInfo.IsUnicode})";
            logger.LogCalling(name, dbSetInfo.EntityType, action);
            entityBuilder.Property(columnInfo.PropertyName).IsUnicode(columnInfo.IsUnicode.Value);
        }
    }
}