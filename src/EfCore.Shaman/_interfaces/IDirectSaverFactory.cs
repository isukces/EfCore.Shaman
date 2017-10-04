using System;

namespace EfCore.Shaman
{
    public interface IDirectSaverFactory : IShamanService
    {
        IDirectSaver<T> GetDirectSaver<T>(Type contextType, Func<ShamanOptions> optionsFactory = null);
    }
}