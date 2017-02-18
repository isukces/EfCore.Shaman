#region using

using System;

#endregion

namespace EfCore.Shaman.ModelScanner
{
    public class ColumnIndexInfo
    {
        #region Properties

        public string IndexName { get; set; }
        public int Order { get; set; }
        public bool IsDescending { get; set; }


        public IndexType IndexType { get; set; }
        public string FullTextCatalogName { get; set; }

        #endregion
    }
}