using System;
using System.Reflection;
using EfCore.Shaman.ModelScanner;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EfCore.Shaman.Services
{
    internal class UnicodeTextAttributeUpdater : IColumnInfoUpdateService
    {
        public void UpdateColumnInfoInModelInfo(ColumnInfo columnInfo, PropertyInfo propertyInfo,
            IDbSetInfo dbSetInfo, IShamanLogger logger)
        {
            var indexAttribute = propertyInfo.GetCustomAttribute<UnicodeTextAttribute>();
            if (indexAttribute == null) return;
            logger.Log(typeof(UnicodeTextAttribute), nameof(UpdateColumnInfoForMigrationFixer),
                $"Set IsUnicode={indexAttribute.IsUnicode}");
            columnInfo.IsUnicode = indexAttribute.IsUnicode;
        }

        public void UpdateColumnInfoForMigrationFixer(ISimpleModelInfo modelInfo, IDbSetInfo dbSetInfo, ColumnInfo columnInfo,
            EntityTypeBuilder entityBuilder,
            IShamanLogger logger)
        {
        }
    }
}