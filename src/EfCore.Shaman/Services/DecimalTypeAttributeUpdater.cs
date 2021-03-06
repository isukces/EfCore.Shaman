﻿using System;
using System.Reflection;
using EfCore.Shaman.ModelScanner;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EfCore.Shaman.Services
{
    internal class DecimalTypeAttributeUpdater : IColumnInfoUpdateService
    {
        public void UpdateColumnInfoOnModelCreating(IDbSetInfo dbSetInfo, ColumnInfo columnInfo,
            EntityTypeBuilder entityBuilder,
            IShamanLogger logger)
        {
        }

        public void UpdateColumnInfoInModelInfo(ColumnInfo columnInfo,
            IDbSetInfo dbSetInfo, IShamanLogger logger)
        {
            var indexAttribute = columnInfo.ClrProperty?.GetCustomAttribute<DecimalTypeAttribute>();
            if (indexAttribute == null) return;
            logger.Log(typeof(DecimalTypeAttributeUpdater), nameof(UpdateColumnInfoOnModelCreating),
                $"Set MaxLength={indexAttribute.Length}, DecimalPlaces={indexAttribute.DecimalPlaces} for column '{columnInfo.ColumnName}'");
            columnInfo.MaxLength = indexAttribute.Length;
            columnInfo.DecimalPlaces = indexAttribute.DecimalPlaces;
        }
    }
}