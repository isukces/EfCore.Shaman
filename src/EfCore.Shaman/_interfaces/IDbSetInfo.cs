using System;

namespace EfCore.Shaman
{
    public interface IDbSetInfo : IShamanAnnotatable, IFullTableName
    {
        Type EntityType { get; }
    }
}