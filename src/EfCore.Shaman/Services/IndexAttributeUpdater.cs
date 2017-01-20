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
                var indexName = indexAttribute.Name?.Trim() ?? "";
                var indexInfo = columnInfo.ColumnIndexes.SingleOrDefault(a => a.IndexName == indexName);
                if (indexInfo == null)
                {
                    indexInfo = new ColumnIndexInfo
                    {
                        IndexName = indexName
                    };
                    columnInfo.ColumnIndexes.Add(indexInfo);
                }
                indexInfo.Order = indexAttribute.Order;
                indexInfo.IsDescending = indexAttribute.IsDescending;
                indexInfo.IsUnique = indexAttribute is UniqueIndexAttribute;
            }
        }

        #endregion
    }
}