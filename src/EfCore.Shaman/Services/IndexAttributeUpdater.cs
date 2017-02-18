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

        public void UpdateColumnInfo(ColumnInfo columnInfo, PropertyInfo propertyInfo, IShamanLogger logger)
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
                        IndexName = indexName,
                        FullTextCatalogName = indexAttribute.FullTextCatalogName
                    };
                    columnInfo.ColumnIndexes.Add(indexInfo);
                }
                indexInfo.Order = indexAttribute.Order;
                indexInfo.IsDescending = indexAttribute.IsDescending;
                if (indexAttribute is UniqueIndexAttribute)
                    indexInfo.IndexType = IndexType.UniqueIndex;
                else if (indexAttribute is FullTextIndexAttribute)
                    indexInfo.IndexType = IndexType.FullTextIndex;
                logger.Log(typeof(IndexAttributeUpdater), nameof(UpdateColumnInfo),
                    $"Set indexInfo: Order={indexInfo.Order}, IsDescending={indexInfo.IsDescending}, IndexType={indexInfo.IndexType}");
            }
        }

        #endregion
    }
}