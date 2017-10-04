using System;
using EfCore.Shaman.ModelScanner;

namespace EfCore.Shaman
{
    public interface IDbSetInfoUpdateService : IShamanService
    {
        void UpdateDbSetInfo(DbSetInfo dbSetInfo, Type entityType, Type contextType, IShamanLogger logger);
    }
}