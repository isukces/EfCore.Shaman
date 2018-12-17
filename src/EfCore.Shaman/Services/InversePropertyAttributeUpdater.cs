#region using

using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection;
using EfCore.Shaman.ModelScanner;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

#endregion

namespace EfCore.Shaman.Services
{
    [Obsolete("Use " + nameof(NavigationPropertyAttribute) + " to explicity set or unset navigation property flag")]
    internal class InversePropertyAttributeUpdater : IColumnInfoUpdateService
    {
        public void UpdateColumnInfoInModelInfo(ColumnInfo columnInfo, PropertyInfo propertyInfo,
            IDbSetInfo dbSetInfo, IShamanLogger logger)
        {
            if (propertyInfo.GetCustomAttribute<InversePropertyAttribute>() != null)
                columnInfo.IsNavigationProperty = true;
            // todo log
        }

        public void UpdateColumnInfoForMigrationFixer(ISimpleModelInfo modelInfo, IDbSetInfo dbSetInfo, ColumnInfo columnInfo,
            EntityTypeBuilder entityBuilder,
            IShamanLogger logger)
        {
        }
    }
}