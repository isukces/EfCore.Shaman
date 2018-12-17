using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection;
using EfCore.Shaman.ModelScanner;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EfCore.Shaman.Services
{
    public class ColumnInfoColumnAttributeUpdater : IColumnInfoUpdateService
    {
        public void UpdateColumnInfoInModelInfo(ColumnInfo columnInfo,
            IDbSetInfo dbSetInfo, IShamanLogger logger)
        {
            var attribute = columnInfo.ClrProperty?.GetCustomAttribute<ColumnAttribute>();
            if (attribute == null) return;
            const string logSource = nameof(ColumnInfoColumnAttributeUpdater) + "." + nameof(UpdateColumnInfoInModelInfo);
            if (!string.IsNullOrEmpty(attribute.Name))
            {
                columnInfo.ColumnName = attribute.Name;
                logger.Log(logSource, $"Set {dbSetInfo.TableName}.{columnInfo.ColumnName}.ColumnName='{columnInfo.ColumnName}'");
            }

            if (attribute.Order >= 0)
            {
                columnInfo.ForceFieldOrder = attribute.Order;
                logger.Log(logSource, $"Set {dbSetInfo.TableName}.{columnInfo.ColumnName}.ForceFieldOrder={columnInfo.ForceFieldOrder}");
            }
        }
 
        public void UpdateColumnInfoOnModelCreating(IDbSetInfo dbSetInfo, ColumnInfo columnInfo,
            EntityTypeBuilder entityBuilder,
            IShamanLogger logger)
        {
        }
    }
}
