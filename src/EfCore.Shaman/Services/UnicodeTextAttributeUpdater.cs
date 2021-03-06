﻿using System;
using System.Reflection;
using EfCore.Shaman.ModelScanner;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EfCore.Shaman.Services
{
    internal class UnicodeTextAttributeUpdater : IColumnInfoUpdateService
    {
        public void UpdateColumnInfoInModelInfo(ColumnInfo columnInfo,
            IDbSetInfo dbSetInfo, IShamanLogger logger)
        {
            var indexAttribute = columnInfo.ClrProperty?.GetCustomAttribute<UnicodeTextAttribute>();
            if (indexAttribute == null) return;
            logger.Log(typeof(UnicodeTextAttribute), nameof(UpdateColumnInfoOnModelCreating),
                $"Set IsUnicode={indexAttribute.IsUnicode}");
            columnInfo.IsUnicode = indexAttribute.IsUnicode;
        }

        public void UpdateColumnInfoOnModelCreating(IDbSetInfo dbSetInfo, ColumnInfo columnInfo,
            EntityTypeBuilder entityBuilder,
            IShamanLogger logger)
        {
        }
    }
}