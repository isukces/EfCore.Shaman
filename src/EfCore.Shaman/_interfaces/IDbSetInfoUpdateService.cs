using System;
using EfCore.Shaman.ModelScanner;

namespace EfCore.Shaman
{
    public interface IDbSetInfoUpdateService : IShamanService
    {
        #region Instance Methods

        void UpdateDbSetInfo(DbSetInfo dbSetInfo, Type entityType, Type contextType);

        #endregion
    }
}