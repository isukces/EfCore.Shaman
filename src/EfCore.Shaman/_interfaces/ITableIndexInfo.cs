using System.Collections.Generic;
using EfCore.Shaman.ModelScanner;

namespace EfCore.Shaman
{
    public interface ITableIndexInfo
    {
        #region Properties

        string IndexName { get; }

        IReadOnlyList<ITableIndexFieldInfo> Fields { get; }
        IndexType IndexType { get; }

        #endregion
    }
}