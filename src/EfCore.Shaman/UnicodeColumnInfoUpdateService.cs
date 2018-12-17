using System;
using System.Reflection;
using EfCore.Shaman.ModelScanner;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EfCore.Shaman
{
    public class UnicodeColumnInfoUpdateService : IColumnInfoUpdateService
    {
        public void UpdateColumnInfoInModelInfo(ColumnInfo columnInfo, PropertyInfo propertyInfo,
            IDbSetInfo dbSetInfo, IShamanLogger logger)
        {
        }

        public void UpdateColumnInfoForMigrationFixer(ISimpleModelInfo modelInfo, IDbSetInfo dbSetInfo, ColumnInfo columnInfo,
            EntityTypeBuilder entityBuilder,
            IShamanLogger logger)
        {
            const string name = nameof(UnicodeColumnInfoUpdateService) + "." + nameof(UpdateColumnInfoForMigrationFixer);
            if (columnInfo.IsUnicode == null)
                return;
            var action = $"IsUnicode({columnInfo.IsUnicode})";
            logger.LogFix(name, dbSetInfo.EntityType, action);
            entityBuilder.Property(columnInfo.PropertyName).IsUnicode(columnInfo.IsUnicode.Value);
        }
    }
}