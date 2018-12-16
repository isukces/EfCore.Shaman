using System;
using System.Reflection;
using EfCore.Shaman.ModelScanner;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EfCore.Shaman.Services
{
    internal class DecimalTypeAttributeUpdater : IColumnInfoUpdateService
    {
        public void ModelFixerUpdateColumnInfo(ColumnInfo columnInfo, EntityTypeBuilder entityBuilder, Type entityType,
            IShamanLogger logger)
        {
        }

        public void ModelInfoUpdateColumnInfo(ColumnInfo columnInfo, PropertyInfo propertyInfo, IShamanLogger logger)
        {
            var indexAttribute = propertyInfo.GetCustomAttribute<DecimalTypeAttribute>();
            if (indexAttribute == null) return;
            logger.Log(typeof(DecimalTypeAttributeUpdater), nameof(ModelFixerUpdateColumnInfo),
                $"Set MaxLength={indexAttribute.Length}, DecimalPlaces={indexAttribute.DecimalPlaces} for column '{columnInfo.ColumnName}'");
            columnInfo.MaxLength = indexAttribute.Length;
            columnInfo.DecimalPlaces = indexAttribute.DecimalPlaces;
        }
    }
}