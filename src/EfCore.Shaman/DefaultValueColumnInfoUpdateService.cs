using System;
using System.Reflection;
using EfCore.Shaman.ModelScanner;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EfCore.Shaman
{
    public class DefaultValueColumnInfoUpdateService:IColumnInfoUpdateService
    {
        public void UpdateColumnInfoInModelInfo(ColumnInfo columnInfo,
            IDbSetInfo dbSetInfo, IShamanLogger logger)
        {
        }
      
        public void UpdateColumnInfoOnModelCreating(IDbSetInfo dbSetInfo, ColumnInfo columnInfo,
            EntityTypeBuilder entityBuilder,
            IShamanLogger logger)
        {
            const string source = nameof(DefaultValueColumnInfoUpdateService) + "." + nameof(UpdateColumnInfoOnModelCreating);
            var dv = columnInfo.DefaultValue;
            if (columnInfo.DefaultValue == null) return;
            string action;
            switch (dv.Kind)
            {
                case ValueInfoKind.Clr:
                    action = $"{columnInfo.PropertyName}.HasDefaultValue(\"{columnInfo.DefaultValue.ClrValue}\")";
                    logger.LogCalling(source, dbSetInfo.EntityType, action);
                    entityBuilder.Property(columnInfo.PropertyName).HasDefaultValue(columnInfo.DefaultValue.ClrValue);
                    break;
                case ValueInfoKind.Sql:
                    action = $"{columnInfo.PropertyName}.HasDefaultValueSql(\"{columnInfo.DefaultValue.SqlValue}\")";
                    logger.LogCalling(source, dbSetInfo.EntityType, action);
                    entityBuilder.Property(columnInfo.PropertyName).HasDefaultValueSql(columnInfo.DefaultValue.SqlValue);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
        
    }
}