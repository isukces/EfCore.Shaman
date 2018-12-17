using System.ComponentModel.DataAnnotations;
using System.Reflection;
using EfCore.Shaman.ModelScanner;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EfCore.Shaman.Services
{
    public class RequiredAttributeUpdater : IColumnInfoUpdateService
    {
        public void UpdateColumnInfoForMigrationFixer(ISimpleModelInfo modelInfo, IDbSetInfo dbSetInfo,
            ColumnInfo columnInfo,
            EntityTypeBuilder entityBuilder,
            IShamanLogger logger)
        {
        }

        public void UpdateColumnInfoInModelInfo(ColumnInfo columnInfo, PropertyInfo propertyInfo,
            IDbSetInfo dbSetInfo, IShamanLogger logger)
        {
            var attribute = propertyInfo.GetCustomAttribute<RequiredAttribute>();
            if (attribute == null) return;

            var logPrefix = $"Set {dbSetInfo.TableName}.{columnInfo.ColumnName}";
            const string logSource =
                nameof(RequiredAttributeUpdater) + "." + nameof(UpdateColumnInfoInModelInfo);

            columnInfo.NotNull = true;
            logger.Log(logSource, $"{logPrefix}.NotNull=true");
        }
    }
}