using System;

namespace EfCore.Shaman.ModelScanner
{
    public class ColumnIndexInfo
    {
        public ColumnIndexInfo(string indexName)
        {
            IndexName = indexName?.Trim() ?? throw new ArgumentNullException(nameof(indexName));
        }

        public string IndexName { get; private set; }
        public int Order { get; set; }
        public bool IsDescending { get; set; }
        public IndexType IndexType { get; set; }
        public FullTextCatalogInfo? FullTextCatalog { get; set; }
#if EF200
        /// <summary>
        /// Index filter (trimmed string)
        /// </summary>
        public string Filter
        {
            get { return _filter; }
            set { _filter = value?.Trim(); }
        }
        private string _filter;
#endif
    }
}