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
        public void ModelFixerUpdateColumnInfo(ColumnInfo columnInfo, EntityTypeBuilder entityBuilder, Type entityType,
            IShamanLogger logger)
        {
        }

        public void ModelInfoUpdateColumnInfo(ColumnInfo columnInfo, PropertyInfo propertyInfo, IShamanLogger logger)
        {
            var attribute = propertyInfo.GetCustomAttribute<DefaultValueAttribute>();
            if (attribute != null)
                columnInfo.DefaultValue = ValueInfo.FromClrValue(attribute.Value);
            // todo: log
        }
    }
}