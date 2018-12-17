using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection;
using EfCore.Shaman.ModelScanner;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EfCore.Shaman.Services
{
    public class ColumnInfoColumnAttributeUpdater : IColumnInfoUpdateService
    {
        public void UpdateColumnInfoInModelInfo(ColumnInfo columnInfo, PropertyInfo propertyInfo,
            IDbSetInfo dbSetInfo, IShamanLogger logger)
        {
            var attribute = propertyInfo.GetCustomAttribute<ColumnAttribute>();
            if (attribute == null) return;
            if (!string.IsNullOrEmpty(attribute.Name))
            {
                columnInfo.ColumnName = attribute.Name;
                logger.Log(typeof(ColumnInfoColumnAttributeUpdater), "UpdateColumnInfo",
                    $"Set ColumnName='{columnInfo.ColumnName}'");
            }
            if (attribute.Order >= 0)
            {
                columnInfo.ForceFieldOrder = attribute.Order;
                logger.Log(typeof(ColumnInfoColumnAttributeUpdater), "UpdateColumnInfo",
                    $"Set ForceFieldOrder={columnInfo.ForceFieldOrder}");
            }
        }
 
        public void UpdateColumnInfoForMigrationFixer(ISimpleModelInfo modelInfo, IDbSetInfo dbSetInfo, ColumnInfo columnInfo,
            EntityTypeBuilder entityBuilder,
            IShamanLogger logger)
        {
        }
    }
}
