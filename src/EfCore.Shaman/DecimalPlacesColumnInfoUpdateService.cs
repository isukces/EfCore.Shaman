using System;
using System.Reflection;
using EfCore.Shaman.ModelScanner;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EfCore.Shaman
{
    public class DecimalPlacesColumnInfoUpdateService : IColumnInfoUpdateService
    {
        public void ModelInfoUpdateColumnInfo(ColumnInfo columnInfo, PropertyInfo propertyInfo, IShamanLogger logger)
        {
        }

        public void ModelFixerUpdateColumnInfo(ColumnInfo columnInfo, EntityTypeBuilder entityBuilder, Type entityType,
            IShamanLogger logger)
        {
            const string name = nameof(DecimalPlacesColumnInfoUpdateService) + "." + nameof(ModelFixerUpdateColumnInfo);
            if (columnInfo.MaxLength == null || columnInfo.DecimalPlaces == null)
                return;
            var type   = $"decimal({columnInfo.MaxLength},{columnInfo.DecimalPlaces})";
            var action = $"HasColumnType(\"{type}\")";
            logger.LogFix(name, entityType, action);
            entityBuilder.Property(columnInfo.PropertyName).HasColumnType(type);
        }
    }
}