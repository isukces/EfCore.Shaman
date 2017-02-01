using System.Collections.Generic;

namespace EfCore.Shaman
{
    /// <summary>
    /// Creates objects used as constructor parameters when DbContext is created by shaman.
    /// </summary>
    public interface IValueProviderService : IShamanService
    {
        IEnumerable<object> CreateObjects();
    }
}
