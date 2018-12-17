using System;
using System.Reflection;
using EfCore.Shaman.ModelScanner;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EfCore.Shaman.Services
{
    public  class NavigationPropertyAttributeUpdater : IColumnInfoUpdateService
    {
        public void UpdateColumnInfoInModelInfo(ColumnInfo columnInfo, PropertyInfo propertyInfo, IShamanLogger logger)
        {
            var attribute = propertyInfo.GetCustomAttribute<NavigationPropertyAttribute>();
            if (attribute == null) return;
            Action<string> log =
                txt => logger.Log(typeof(NavigationPropertyAttributeUpdater), nameof(UpdateColumnInfoForMigrationFixer), txt);
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

        public void UpdateColumnInfoForMigrationFixer(ISimpleModelInfo modelInfo, IDbSetInfo dbSetInfo, ColumnInfo columnInfo,
            EntityTypeBuilder entityBuilder,
            IShamanLogger logger)
        {            
        }
    }
}