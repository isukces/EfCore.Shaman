#region using

using System;
using System.Reflection;
using EfCore.Shaman.ModelScanner;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

#endregion

namespace EfCore.Shaman.Services
{
    internal class DefaultValueSqlAttributeUpdater : IColumnInfoUpdateService
    {

        public void UpdateColumnInfoInModelInfo(ColumnInfo columnInfo,
            IDbSetInfo dbSetInfo, IShamanLogger logger)
        {
            var attribute = columnInfo.ClrProperty?.GetCustomAttribute<DefaultValueSqlAttribute>();
            if (!string.IsNullOrEmpty(attribute?.DefaultValueSql))
                columnInfo.DefaultValue = ValueInfo.FromSqlValue(attribute.DefaultValueSql);
            // todo: log
        }

        public void UpdateColumnInfoOnModelCreating(IDbSetInfo dbSetInfo, ColumnInfo columnInfo,
            EntityTypeBuilder entityBuilder,
            IShamanLogger logger)
        {            
        }
    }
}