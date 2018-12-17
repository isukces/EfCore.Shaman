using System;
using System.Reflection;
using EfCore.Shaman.ModelScanner;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EfCore.Shaman.Services
{
    internal class DecimalTypeAttributeUpdater : IColumnInfoUpdateService
    {
        public void UpdateColumnInfoForMigrationFixer(ISimpleModelInfo modelInfo, IDbSetInfo dbSetInfo, ColumnInfo columnInfo,
            EntityTypeBuilder entityBuilder,
            IShamanLogger logger)
        {
        }

        public void UpdateColumnInfoInModelInfo(ColumnInfo columnInfo, PropertyInfo propertyInfo,
            IDbSetInfo dbSetInfo, IShamanLogger logger)
        {
            var indexAttribute = propertyInfo.GetCustomAttribute<DecimalTypeAttribute>();
            if (indexAttribute == null) return;
            logger.Log(typeof(DecimalTypeAttributeUpdater), nameof(UpdateColumnInfoForMigrationFixer),
                $"Set MaxLength={indexAttribute.Length}, DecimalPlaces={indexAttribute.DecimalPlaces} for column '{columnInfo.ColumnName}'");
            columnInfo.MaxLength = indexAttribute.Length;
            columnInfo.DecimalPlaces = indexAttribute.DecimalPlaces;
        }
    }
}