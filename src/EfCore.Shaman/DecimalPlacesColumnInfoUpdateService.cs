using System;
using System.Reflection;
using EfCore.Shaman.ModelScanner;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EfCore.Shaman
{
    public class DecimalPlacesColumnInfoUpdateService : IColumnInfoUpdateService
    {
        public void UpdateColumnInfoInModelInfo(ColumnInfo columnInfo,
            IDbSetInfo dbSetInfo, IShamanLogger logger)
        {
        }

        public void UpdateColumnInfoOnModelCreating(IDbSetInfo dbSetInfo, ColumnInfo columnInfo,
            EntityTypeBuilder entityBuilder,
            IShamanLogger logger)
        {
            const string name = nameof(DecimalPlacesColumnInfoUpdateService) + "." + nameof(UpdateColumnInfoOnModelCreating);
            if (columnInfo.MaxLength == null || columnInfo.DecimalPlaces == null)
                return;
            var type   = $"decimal({columnInfo.MaxLength},{columnInfo.DecimalPlaces})";
            var action = $"{columnInfo.PropertyName}.HasColumnType(\"{type}\")";
            logger.LogCalling(name, dbSetInfo.EntityType, action);
            entityBuilder.Property(columnInfo.PropertyName).HasColumnType(type);
        }
    }
}