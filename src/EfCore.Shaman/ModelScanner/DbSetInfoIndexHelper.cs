#region using

using System;
using System.Collections.Generic;
using System.Linq;

#endregion

namespace EfCore.Shaman.ModelScanner
{
    internal static class DbSetInfoIndexHelper
    {
        #region Static Methods

        public static ITableIndexInfo CreateTableIndexInfo(IGrouping<string, DbSetInfo.ColumnAndIndex> grouping,
            string tableName)
        {
            var list = grouping.ToList();
            var indexInfo = new TableIndexInfo(grouping.Key)
            {
                Fields = list.OrderBy(q => q.IndexInfo.Order)
                    .Select(q => new TableIndexFieldInfo
                    {
                        FieldName = q.Column.ColumnName,
                        IsDescending = q.IndexInfo.IsDescending
                    }).ToArray(),
                IndexType = AggregateIndexType(list),
            };
            if (indexInfo.IndexType == IndexType.FullTextIndex)
                indexInfo.FullTextCatalogName = AggregateFullTextCatalogName(list, tableName);
#if EF200
            var filters = list
                .Select(a => a.IndexInfo.Filter)
                .Where(a => !string.IsNullOrEmpty(a))
                .Distinct()
                .ToArray();
            if (filters.Length > 1)
                throw new InvalidIndexFilterException(
                    $"Two or more different filters for single index i.e. '{filters[0]}' and '{filters[1]}'",
                    (ITableIndexInfo)indexInfo);
            indexInfo.Filter = filters.FirstOrDefault();
#endif

            return indexInfo;
        }

        private static string AggregateFullTextCatalogName(List<DbSetInfo.ColumnAndIndex> list, string tableName)
        {
            var distinctNames = list
                .Select(a => a.IndexInfo.FullTextCatalog?.Name?.Trim())
                .Where(a => !string.IsNullOrEmpty(a))
                .Distinct()
                .ToArray();
            switch (distinctNames.Length)
            {
                case 0:
                    throw new Exception("Unable to find FullTextCatalogName for table " + tableName);
                case 1:
                    return distinctNames[0];
                default:
                    throw new Exception("To many different FullTextCatalogNames for table " + tableName);
            }
        }

        private static IndexType AggregateIndexType(IEnumerable<DbSetInfo.ColumnAndIndex> list)
        {
            var configuration = IndexTypeConfiguration.None;
            foreach (var columnAndIndex in list)
            {
                switch (columnAndIndex.IndexInfo.IndexType)
                {
                    case IndexType.FullTextIndex:
                        configuration |= IndexTypeConfiguration.FullTextIndex;
                        break;
                    case IndexType.UniqueIndex:
                        configuration |= IndexTypeConfiguration.UniqueIndex;
                        break;
                }
                if (configuration == IndexTypeConfiguration.Forbidden)
                    throw new Exception("Forbidden index type combination for index " +
                                        columnAndIndex.IndexInfo.IndexName);
            }

            switch (configuration)
            {
                case IndexTypeConfiguration.None:
                    return IndexType.Index;
                case IndexTypeConfiguration.FullTextIndex:
                    return IndexType.FullTextIndex;
                case IndexTypeConfiguration.UniqueIndex:
                    return IndexType.UniqueIndex;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        #endregion

        #region Enums

        [Flags]
        private enum IndexTypeConfiguration
        {
            None = 0,
            FullTextIndex = 1,
            UniqueIndex = 2,
            Forbidden = 3
        }

        #endregion
    }
}