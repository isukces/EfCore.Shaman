using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;

namespace EfCore.Shaman.ModelScanner
{
    public class DbSetInfo : IDbSetInfo
    {
        public DbSetInfo(Type entityType, string tableName, [NotNull] ISimpleModelInfo model, string schema = "")
        {
            EntityType = entityType;
            TableName  = tableName;
            Model      = model ?? throw new ArgumentNullException(nameof(model));
            Schema     = schema;
        }

        public override string ToString()
        {
            return $"DbSet: {EntityType?.Name} => {Schema}.{TableName}";
        }

        public string           TableName  { get; set; }
        public string           Schema     { get; set; }
        public Type             EntityType { get; }
        public ISimpleModelInfo Model      { get; }
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


        public class ColumnAndIndex
        {
            public ColumnAndIndex(ColumnInfo column, ColumnIndexInfo indexInfo)
            {
                Column    = column;
                IndexInfo = indexInfo;
            }

            public ColumnInfo      Column    { get; }
            public ColumnIndexInfo IndexInfo { get; }
        }
    }
}