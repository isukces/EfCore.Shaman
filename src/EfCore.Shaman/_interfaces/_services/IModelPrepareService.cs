using System;

namespace EfCore.Shaman
{
    public interface IModelPrepareService :IShamanService
    {
        void UpdateModel(IUpdatableSimpleModelInfo simpleModelInfo, Type dbContextType, IShamanLogger logger);
    }
}