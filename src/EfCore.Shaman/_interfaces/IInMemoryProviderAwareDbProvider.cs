namespace EfCore.Shaman
{
    /// <summary>
    /// <see cref="http://efcoreshaman.com/inmemory-provider-troubles.html">InMemory db provider</see>
    /// </summary>
    public interface IInMemoryDatabaseAwareDbProvider
    {
        /// <summary>
        /// <see cref="http://efcoreshaman.com/inmemory-provider-troubles.html">InMemory db provider</see>
        /// </summary>
        bool IsUsingInMemoryDatabase { get; }
    }
}
