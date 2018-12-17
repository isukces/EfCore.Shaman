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
        public void UpdateColumnInfoInModelInfo(ColumnInfo columnInfo,
            IDbSetInfo dbSetInfo, IShamanLogger logger)
        {
            if (columnInfo.ClrProperty?.GetCustomAttribute<ForeignKeyAttribute>() != null)
                columnInfo.IsNavigationProperty = true;
            // todo log
            // todo does ForeignKeyAttribute always means IsNavigationProperty = true ?
        }

        public void UpdateColumnInfoOnModelCreating(IDbSetInfo dbSetInfo, ColumnInfo columnInfo,
            EntityTypeBuilder entityBuilder,
            IShamanLogger logger)
        {
        }
    }
}