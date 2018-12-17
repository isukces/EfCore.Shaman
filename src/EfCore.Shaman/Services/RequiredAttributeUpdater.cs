using System;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
using EfCore.Shaman.ModelScanner;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EfCore.Shaman.Services
{
    public class RequiredAttributeUpdater : IColumnInfoUpdateService
    {
        public void UpdateColumnInfoInModelInfo(ColumnInfo columnInfo, PropertyInfo propertyInfo,
            IDbSetInfo dbSetInfo, IShamanLogger logger)
        {
            var attribute = propertyInfo.GetCustomAttribute<RequiredAttribute>();
            if (attribute == null) return;
            columnInfo.NotNull = true;
            logger.Log(typeof(RequiredAttributeUpdater), nameof(UpdateColumnInfoForMigrationFixer),
                $"Set NotNull=true for column '{columnInfo.ColumnName}'");
        }

        public void UpdateColumnInfoForMigrationFixer(ISimpleModelInfo modelInfo, IDbSetInfo dbSetInfo, ColumnInfo columnInfo,
            EntityTypeBuilder entityBuilder,
            IShamanLogger logger)
        {            
        }
    }
}
