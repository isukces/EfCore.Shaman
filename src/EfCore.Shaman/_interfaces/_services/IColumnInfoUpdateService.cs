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
        /// <param name="dbSetInfo"></param>
        /// <param name="logger"></param>
        void UpdateColumnInfoInModelInfo(ColumnInfo columnInfo,
            IDbSetInfo dbSetInfo, IShamanLogger logger);

        /// <summary>
        /// This method is used when DbContext.OnModelCreating is executed
        /// </summary>
        /// <param name="dbSetInfo"></param>
        /// <param name="columnInfo"></param>
        /// <param name="entityBuilder"></param>
        /// <param name="logger"></param>
        void UpdateColumnInfoOnModelCreating(IDbSetInfo dbSetInfo, ColumnInfo columnInfo,
            EntityTypeBuilder entityBuilder, IShamanLogger logger);
    }
}