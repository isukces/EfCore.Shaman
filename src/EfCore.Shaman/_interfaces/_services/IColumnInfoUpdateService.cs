using System;
using System.Reflection;
using EfCore.Shaman.ModelScanner;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EfCore.Shaman
{
    public interface IColumnInfoUpdateService : IShamanService
    {
        /// <summary>
        /// This method is used when ModelInfo is created
        /// </summary>
        /// <param name="columnInfo"></param>
        /// <param name="propertyInfo"></param>
        /// <param name="dbSetInfo"></param>
        /// <param name="logger"></param>
        void UpdateColumnInfoInModelInfo(ColumnInfo columnInfo, PropertyInfo propertyInfo,
            IDbSetInfo dbSetInfo, IShamanLogger logger);

        /// <summary>
        /// This method is used when migration is being fixed
        /// </summary>
        /// <param name="modelInfo"></param>
        /// <param name="dbSetInfo"></param>
        /// <param name="columnInfo"></param>
        /// <param name="entityBuilder"></param>
        /// <param name="logger"></param>
        void UpdateColumnInfoForMigrationFixer(ISimpleModelInfo modelInfo, IDbSetInfo dbSetInfo, ColumnInfo columnInfo,
            EntityTypeBuilder entityBuilder,
            IShamanLogger logger);
    }
}