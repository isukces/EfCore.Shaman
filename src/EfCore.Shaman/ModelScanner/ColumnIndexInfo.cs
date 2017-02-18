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

        [Obsolete]
        public bool IsUnique => IndexType == ModelScanner.IndexType.UniqueIndex;

        public IndexType IndexType { get; set; }

        #endregion
    }
}