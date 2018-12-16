using System;
using System.Reflection;
using EfCore.Shaman.ModelScanner;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EfCore.Shaman
{
    public class UnicodeColumnInfoUpdateService : IColumnInfoUpdateService
    {
        public void ModelInfoUpdateColumnInfo(ColumnInfo columnInfo, PropertyInfo propertyInfo, IShamanLogger logger)
        {
        }

        public void ModelFixerUpdateColumnInfo(ColumnInfo columnInfo, EntityTypeBuilder entityBuilder, Type entityType,
            IShamanLogger logger)
        {
            const string name = nameof(UnicodeColumnInfoUpdateService) + "." + nameof(ModelFixerUpdateColumnInfo);
            if (columnInfo.IsUnicode == null)
                return;
            var action = $"IsUnicode({columnInfo.IsUnicode})";
            logger.LogFix(name, entityType, action);
            entityBuilder.Property(columnInfo.PropertyName).IsUnicode(columnInfo.IsUnicode.Value);
        }
    }
}