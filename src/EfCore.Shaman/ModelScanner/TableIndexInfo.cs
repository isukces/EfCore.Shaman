using System;
using System.Collections.Generic;

namespace EfCore.Shaman.ModelScanner
{
    internal class TableIndexInfo : ITableIndexInfo
    {
        public TableIndexInfo(string indexName)
        {
            IndexName = indexName ?? throw new ArgumentNullException(nameof(indexName));
        }

        public string IndexName { get; private set; }
        public IReadOnlyList<ITableIndexFieldInfo> Fields { get; set; } = new List<ITableIndexFieldInfo>();
        public IndexType IndexType { get; set; }
        public string FullTextCatalogName { get; set; }
#if EF200
        public string Filter { get; set; }
#endif
    }
}