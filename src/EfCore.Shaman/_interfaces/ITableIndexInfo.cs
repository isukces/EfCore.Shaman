using System.Collections.Generic;
using EfCore.Shaman.ModelScanner;

namespace EfCore.Shaman
{
    public interface ITableIndexInfo
    {
        string IndexName { get; }

        IReadOnlyList<ITableIndexFieldInfo> Fields { get; }
        IndexType IndexType { get; }
        string FullTextCatalogName { get;  }
#if EF200
        string Filter { get; }
#endif
    }
}