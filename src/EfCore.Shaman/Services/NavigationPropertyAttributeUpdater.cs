#region using

using System;
using System.Reflection;
using EfCore.Shaman.ModelScanner;

#endregion

namespace EfCore.Shaman.Services
{
    internal class NavigationPropertyAttributeUpdater : IColumnInfoUpdateService
    {
        #region Instance Methods

        public void UpdateColumnInfo(ColumnInfo columnInfo, PropertyInfo propertyInfo, IShamanLogger logger)
        {
            var attribute = propertyInfo.GetCustomAttribute<NavigationPropertyAttribute>();
            if (attribute == null) return;
            Action<string> log =
                txt => logger.Log(typeof(NavigationPropertyAttributeUpdater), nameof(UpdateColumnInfo), txt);
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

        #endregion
    }
}