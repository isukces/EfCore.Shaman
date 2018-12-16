using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection;
using EfCore.Shaman.ModelScanner;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EfCore.Shaman.Services
{
    class ColumnAttributeUpdater : IColumnInfoUpdateService
    {
        public void ModelInfoUpdateColumnInfo(ColumnInfo columnInfo, PropertyInfo propertyInfo, IShamanLogger logger)
        {
            var attribute = propertyInfo.GetCustomAttribute<ColumnAttribute>();
            if (attribute == null) return;
            if (!string.IsNullOrEmpty(attribute.Name))
            {
                columnInfo.ColumnName = attribute.Name;
                logger.Log(typeof(ColumnAttributeUpdater), "UpdateColumnInfo",
                    $"Set ColumnName='{columnInfo.ColumnName}'");
            }
            if (attribute.Order >= 0)
            {
                columnInfo.ForceFieldOrder = attribute.Order;
                logger.Log(typeof(ColumnAttributeUpdater), "UpdateColumnInfo",
                    $"Set ForceFieldOrder={columnInfo.ForceFieldOrder}");
            }
        }

        public void ModelFixerUpdateColumnInfo(ColumnInfo columnInfo, EntityTypeBuilder entityBuilder, Type entityType,
            IShamanLogger logger)
        {
        }
    }
}
