using System;
using System.Reflection;
using EfCore.Shaman.ModelScanner;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EfCore.Shaman
{
    public class DecimalPlacesColumnInfoUpdateService : IColumnInfoUpdateService
    {
        public void UpdateColumnInfoInModelInfo(ColumnInfo columnInfo, PropertyInfo propertyInfo,
            IDbSetInfo dbSetInfo, IShamanLogger logger)
        {
        }

        public void UpdateColumnInfoForMigrationFixer(ISimpleModelInfo modelInfo, IDbSetInfo dbSetInfo, ColumnInfo columnInfo,
            EntityTypeBuilder entityBuilder,
            IShamanLogger logger)
        {
            const string name = nameof(DecimalPlacesColumnInfoUpdateService) + "." + nameof(UpdateColumnInfoForMigrationFixer);
            if (columnInfo.MaxLength == null || columnInfo.DecimalPlaces == null)
                return;
            var type   = $"decimal({columnInfo.MaxLength},{columnInfo.DecimalPlaces})";
            var action = $"HasColumnType(\"{type}\")";
            logger.LogFix(name, dbSetInfo.EntityType, action);
            entityBuilder.Property(columnInfo.PropertyName).HasColumnType(type);
        }
    }
}