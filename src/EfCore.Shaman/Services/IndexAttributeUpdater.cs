#region using

using System.Linq;
using System.Reflection;
using EfCore.Shaman.ModelScanner;

#endregion

namespace EfCore.Shaman.Services
{
    internal class IndexAttributeUpdater : IColumnInfoUpdateService
    {
        #region Instance Methods

        public void UpdateColumnInfo(ColumnInfo columnInfo, PropertyInfo propertyInfo)
        {
            var indexAttributes = propertyInfo.GetCustomAttributes<AbstractIndexAttribute>()?.ToArray();
            if ((indexAttributes == null) || !indexAttributes.Any()) return;
            foreach (var indexAttribute in indexAttributes)
            {
                var indexMember = new ColumnIndexInfo
                {
                    IndexName = indexAttribute.Name?.Trim(),
                    Order = indexAttribute.Order,
                    IsDescending = indexAttribute.IsDescending,
                    IsUnique = indexAttribute is UniqueIndexAttribute
                };
                columnInfo.ColumnIndexes.Add(indexMember);
            }
        }

        #endregion
    }
}