using System;
using JetBrains.Annotations;

namespace EfCore.Shaman
{
    public interface IDbSetInfo : IShamanAnnotatable, IFullTableName
    {
        Type EntityType { get; }
        
        [NotNull]
        ISimpleModelInfo Model { get; }
    }
}