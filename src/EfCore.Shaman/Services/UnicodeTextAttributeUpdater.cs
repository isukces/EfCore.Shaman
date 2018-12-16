using System;
using System.Reflection;
using EfCore.Shaman.ModelScanner;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EfCore.Shaman.Services
{
    internal class UnicodeTextAttributeUpdater : IColumnInfoUpdateService
    {
        public void ModelInfoUpdateColumnInfo(ColumnInfo columnInfo, PropertyInfo propertyInfo, IShamanLogger logger)
        {
            var indexAttribute = propertyInfo.GetCustomAttribute<UnicodeTextAttribute>();
            if (indexAttribute == null) return;
            logger.Log(typeof(UnicodeTextAttribute), nameof(ModelFixerUpdateColumnInfo),
                $"Set IsUnicode={indexAttribute.IsUnicode}");
            columnInfo.IsUnicode = indexAttribute.IsUnicode;
        }

        public void ModelFixerUpdateColumnInfo(ColumnInfo columnInfo, EntityTypeBuilder entityBuilder, Type entityType,
            IShamanLogger logger)
        {
        }
    }
}