using System;
using System.Reflection;
using EfCore.Shaman.ModelScanner;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EfCore.Shaman
{
    public class DefaultValueColumnInfoUpdateService:IColumnInfoUpdateService
    {
        public void ModelInfoUpdateColumnInfo(ColumnInfo columnInfo, PropertyInfo propertyInfo, IShamanLogger logger)
        {
        }
      
        public void ModelFixerUpdateColumnInfo(ColumnInfo columnInfo, EntityTypeBuilder entityBuilder, Type entityType,
            IShamanLogger logger)
        {
            const string name = nameof(DefaultValueColumnInfoUpdateService) + "." + nameof(ModelFixerUpdateColumnInfo);
            var dv = columnInfo.DefaultValue;
            if (columnInfo.DefaultValue == null) return;
            string action;
            switch (dv.Kind)
            {
                case ValueInfoKind.Clr:
                    action = $"HasDefaultValue(\"{columnInfo.DefaultValue.ClrValue}\")";
                    logger.LogFix(name, entityType, action);
                    entityBuilder.Property(columnInfo.PropertyName).HasDefaultValue(columnInfo.DefaultValue.ClrValue);
                    break;
                case ValueInfoKind.Sql:
                    action = $"HasDefaultValueSql(\"{columnInfo.DefaultValue.SqlValue}\")";
                    logger.LogFix(name, entityType, action);
                    entityBuilder.Property(columnInfo.PropertyName).HasDefaultValueSql(columnInfo.DefaultValue.SqlValue);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
        
    }
}