using System;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
using EfCore.Shaman.ModelScanner;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EfCore.Shaman.Services
{
    class RequiredAttributeUpdater : IColumnInfoUpdateService
    {
        public void ModelInfoUpdateColumnInfo(ColumnInfo columnInfo, PropertyInfo propertyInfo, IShamanLogger logger)
        {
            var attribute = propertyInfo.GetCustomAttribute<RequiredAttribute>();
            if (attribute == null) return;
            columnInfo.NotNull = true;
            logger.Log(typeof(RequiredAttributeUpdater), nameof(ModelFixerUpdateColumnInfo),
                $"Set NotNull=true for column '{columnInfo.ColumnName}'");
        }

        public void ModelFixerUpdateColumnInfo(ColumnInfo columnInfo, EntityTypeBuilder entityBuilder, Type entityType,
            IShamanLogger logger)
        {            
        }
    }
}
