#region using

using System;
using System.Collections.Generic;
using System.Linq;

#endregion

namespace EfCore.Shaman.ModelScanner
{
    public class DbSetInfo : IFullTableName
    {
        #region Constructors

        public DbSetInfo(Type entityType, string tableName, string schema = "")
        {
            EntityType = entityType;
            TableName = tableName;
            Schema = schema;
        }

        #endregion

        #region Static Methods

        private static IndexType AggregateIndexType(IEnumerable<ColumnAndIndex> list)
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
                    throw new Exception("Forbidden index type combination for index " + columnAndIndex.IndexInfo.IndexName);
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

        #region Instance Methods

        public override string ToString() => $"DbSet: {EntityType?.Name} => {Schema}.{TableName}";

        #endregion

        #region Properties

        public string TableName { get; set; }
        public string Schema { get; set; }
        public Type EntityType { get; private set; }
        public List<ColumnInfo> Properites { get; } = new List<ColumnInfo>();

        public IDictionary<string, object> Annotations { get; } =
            new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);

        public IReadOnlyList<ITableIndexInfo> Indexes
        {
            get
            {
                var query = from column in Properites
                            from indexInfo in column.ColumnIndexes
                            group new ColumnAndIndex(column, indexInfo)
                            by indexInfo.IndexName
                    into g
                            let ii = new TableIndexInfo
                            {
                                IndexName = g.Key,
                                Fields = g.OrderBy(q => q.IndexInfo.Order)
                                    .Select(q => new TableIndexFieldInfo
                                    {
                                        FieldName = q.Column.ColumnName,
                                        IsDescending = q.IndexInfo.IsDescending
                                    }).ToArray(),
                                IndexType = AggregateIndexType(g)
                            }
                            select (ITableIndexInfo)ii;
                return query.ToList();
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

        #region Nested

        public class ColumnAndIndex
        {
            #region Constructors

            public ColumnAndIndex(ColumnInfo column, ColumnIndexInfo indexInfo)
            {
                Column = column;
                IndexInfo = indexInfo;
            }

            #endregion

            #region Properties

            public ColumnInfo Column { get; }
            public ColumnIndexInfo IndexInfo { get; }

            #endregion
        }

        #endregion
    }
}