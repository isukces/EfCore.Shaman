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
        /// <param name="logger"></param>
        void ModelInfoUpdateColumnInfo(ColumnInfo columnInfo, PropertyInfo propertyInfo, IShamanLogger logger);
        
        /// <summary>
        /// This method is used when migration is being fixed
        /// </summary>
        /// <param name="columnInfo"></param>
        /// <param name="entityBuilder"></param>
        /// <param name="entityType"></param>
        /// <param name="logger"></param>
        void ModelFixerUpdateColumnInfo(ColumnInfo columnInfo, EntityTypeBuilder entityBuilder, Type entityType,
            IShamanLogger logger);
    }
}