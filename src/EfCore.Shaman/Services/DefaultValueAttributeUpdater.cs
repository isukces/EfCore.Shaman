#region using

using System;
using System.ComponentModel;
using System.Reflection;
using EfCore.Shaman.ModelScanner;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

#endregion

namespace EfCore.Shaman.Services
{
    internal class DefaultValueAttributeUpdater : IColumnInfoUpdateService
    {
        public void UpdateColumnInfoOnModelCreating(IDbSetInfo dbSetInfo, ColumnInfo columnInfo,
            EntityTypeBuilder entityBuilder,
            IShamanLogger logger)
        {
        }

        public void UpdateColumnInfoInModelInfo(ColumnInfo columnInfo,
            IDbSetInfo dbSetInfo, IShamanLogger logger)
        {
            var attribute = columnInfo.ClrProperty?.GetCustomAttribute<DefaultValueAttribute>();
            if (attribute != null)
                columnInfo.DefaultValue = ValueInfo.FromClrValue(attribute.Value);
            // todo: log
        }
    }
}