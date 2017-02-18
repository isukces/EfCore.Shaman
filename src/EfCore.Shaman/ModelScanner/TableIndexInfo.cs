#region using

using System.Collections.Generic;

#endregion

namespace EfCore.Shaman.ModelScanner
{
    internal class TableIndexInfo : ITableIndexInfo
    {
        #region Properties

        public string IndexName { get; set; }
        public IReadOnlyList<ITableIndexFieldInfo> Fields { get; set; } = new List<ITableIndexFieldInfo>();
        public IndexType IndexType { get; set; }
        public string FullTextCatalogName { get; set; }

        #endregion
    }
}