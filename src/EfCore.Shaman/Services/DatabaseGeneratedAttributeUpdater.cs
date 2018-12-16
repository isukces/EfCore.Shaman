#region using

using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection;
using EfCore.Shaman.ModelScanner;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

#endregion

namespace EfCore.Shaman.Services
{
    internal class DatabaseGeneratedAttributeUpdater : IColumnInfoUpdateService
    {
        public void ModelInfoUpdateColumnInfo(ColumnInfo columnInfo, PropertyInfo propertyInfo, IShamanLogger logger)
        {
            var at = propertyInfo.GetCustomAttribute<DatabaseGeneratedAttribute>();
            if (at == null)
                return;
            logger.Log(
                typeof(DatabaseGeneratedAttributeUpdater),
                nameof(ModelFixerUpdateColumnInfo),
                $"Set IsDatabaseGenerated=true and DatabaseGeneratedOption.Identity for column {columnInfo.ColumnName}");
            columnInfo.IsDatabaseGenerated = true;
            columnInfo.IsIdentity = at.DatabaseGeneratedOption == DatabaseGeneratedOption.Identity;
        }
        public void ModelFixerUpdateColumnInfo(ColumnInfo columnInfo, EntityTypeBuilder entityBuilder, Type entityType,
            IShamanLogger logger)
        {
        }

    }
}