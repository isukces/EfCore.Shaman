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

        public void UpdateColumnInfoInModelInfo(ColumnInfo columnInfo, PropertyInfo propertyInfo,
            IDbSetInfo dbSetInfo, IShamanLogger logger)
        {
            var attribute = propertyInfo.GetCustomAttribute<DefaultValueSqlAttribute>();
            if (!string.IsNullOrEmpty(attribute?.DefaultValueSql))
                columnInfo.DefaultValue = ValueInfo.FromSqlValue(attribute.DefaultValueSql);
            // todo: log
        }

        public void UpdateColumnInfoForMigrationFixer(ISimpleModelInfo modelInfo, IDbSetInfo dbSetInfo, ColumnInfo columnInfo,
            EntityTypeBuilder entityBuilder,
            IShamanLogger logger)
        {            
        }
    }
}