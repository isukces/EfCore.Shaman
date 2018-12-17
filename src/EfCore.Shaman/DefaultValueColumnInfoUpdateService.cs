using System;
using System.Reflection;
using EfCore.Shaman.ModelScanner;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EfCore.Shaman
{
    public class DefaultValueColumnInfoUpdateService:IColumnInfoUpdateService
    {
        public void UpdateColumnInfoInModelInfo(ColumnInfo columnInfo, PropertyInfo propertyInfo, IShamanLogger logger)
        {
        }
      
        public void UpdateColumnInfoForMigrationFixer(ISimpleModelInfo modelInfo, IDbSetInfo dbSetInfo, ColumnInfo columnInfo,
            EntityTypeBuilder entityBuilder,
            IShamanLogger logger)
        {
            const string source = nameof(DefaultValueColumnInfoUpdateService) + "." + nameof(UpdateColumnInfoForMigrationFixer);
            var dv = columnInfo.DefaultValue;
            if (columnInfo.DefaultValue == null) return;
            string action;
            switch (dv.Kind)
            {
                case ValueInfoKind.Clr:
                    action = $"{columnInfo.PropertyName}.HasDefaultValue(\"{columnInfo.DefaultValue.ClrValue}\")";
                    logger.LogFix(source, dbSetInfo.EntityType, action);
                    entityBuilder.Property(columnInfo.PropertyName).HasDefaultValue(columnInfo.DefaultValue.ClrValue);
                    break;
                case ValueInfoKind.Sql:
                    action = $"{columnInfo.PropertyName}.HasDefaultValueSql(\"{columnInfo.DefaultValue.SqlValue}\")";
                    logger.LogFix(source, dbSetInfo.EntityType, action);
                    entityBuilder.Property(columnInfo.PropertyName).HasDefaultValueSql(columnInfo.DefaultValue.SqlValue);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
        
    }
}