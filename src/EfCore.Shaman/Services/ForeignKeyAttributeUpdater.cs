using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection;
using EfCore.Shaman.ModelScanner;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EfCore.Shaman.Services
{
    [Obsolete("Use " + nameof(NavigationPropertyAttribute) + " to explicity set or unset navigation property flag")]
    internal class ForeignKeyAttributeUpdater : IColumnInfoUpdateService
    {
        public void ModelInfoUpdateColumnInfo(ColumnInfo columnInfo, PropertyInfo propertyInfo, IShamanLogger logger)
        {
            if (propertyInfo.GetCustomAttribute<ForeignKeyAttribute>() != null)
                columnInfo.IsNavigationProperty = true;
            // todo log
            // todo does ForeignKeyAttribute always means IsNavigationProperty = true ?
        }

        public void ModelFixerUpdateColumnInfo(ColumnInfo columnInfo, EntityTypeBuilder entityBuilder, Type entityType,
            IShamanLogger logger)
        {
        }
    }
}