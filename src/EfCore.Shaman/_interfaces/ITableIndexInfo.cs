using System.Collections.Generic;

namespace EfCore.Shaman
{
    public interface ITableIndexInfo
    {
        #region Properties

        string IndexName { get; }

        IReadOnlyList<ITableIndexFieldInfo> Fields { get; }
        bool IsUnique { get; }

        #endregion
    }
}