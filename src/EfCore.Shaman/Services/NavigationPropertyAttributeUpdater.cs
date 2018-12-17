using System;
using System.Reflection;
using EfCore.Shaman.ModelScanner;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EfCore.Shaman.Services
{
    /// <summary>
    /// Updates IsNavigationProperty based on
    /// <see cref="NavigationPropertyAttribute">NavigationPropertyAttribute</see>
    /// </summary>
    public class NavigationPropertyAttributeUpdater : IColumnInfoUpdateService
    {
        public void UpdateColumnInfoOnModelCreating(IDbSetInfo dbSetInfo,
            ColumnInfo columnInfo,
            EntityTypeBuilder entityBuilder,
            IShamanLogger logger)
        {
        }

        public void UpdateColumnInfoInModelInfo(ColumnInfo columnInfo,
            IDbSetInfo dbSetInfo, IShamanLogger logger)
        {
            var attribute = columnInfo.ClrProperty?.GetCustomAttribute<NavigationPropertyAttribute>();
            if (attribute == null) return;
            Action<string> log =
                txt => logger.Log(typeof(NavigationPropertyAttributeUpdater), nameof(UpdateColumnInfoOnModelCreating),
                    txt);
            var targetType = attribute.ForceNavigation ? "navigation" : "non-navigation";
            if (columnInfo.IsNavigationProperty == attribute.ForceNavigation)
                log(
                    $"column {columnInfo.ColumnName} has been already set as {targetType} property. NavigationPropertyAttribute is not necessary.");
            else
            {
                columnInfo.IsNavigationProperty = attribute.ForceNavigation;
                log($"column {columnInfo.ColumnName} is {targetType} property because of NavigationPropertyAttribute");
            }
        }
    }
}