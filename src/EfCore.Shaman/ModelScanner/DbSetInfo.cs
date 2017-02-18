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
                    select DbSetInfoIndexHelper.CreateTableIndexInfo(g, TableName);
                return query.ToList();
            }
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