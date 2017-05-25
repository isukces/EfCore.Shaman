using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using EfCore.Shaman.Services;
using Microsoft.EntityFrameworkCore;

namespace EfCore.Shaman
{
    public class ShamanDbContext : DbContext, IInMemoryDatabaseAwareDbProvider
    {
        public ShamanDbContext(DbContextOptions options)
            : base(options)
        {
        }

        protected ShamanDbContext()
        {
        }

        /// <summary>
        /// Current db provider is 'inMemory' so many features doesn't work
        /// <see cref="http://efcoreshaman.com/inmemory-provider-troubles.html">InMemory db provider</see>
        /// </summary>

        public bool IsUsingInMemoryDatabase { get; protected set; }


        public IDirectSaver<T> GetDirectSaver<T>(Func<ShamanOptions> optionsFactory = null)
        {
            _lock.EnterUpgradeableReadLock();
            try
            {
                object value;
                if (_directSaverCache.TryGetValue(typeof(T), out value))
                    return (IDirectSaver<T>)value;
                _lock.EnterWriteLock();
                try
                {
                    var options = ShamanOptions.CreateShamanOptions(GetType());
                    var o = options?.Services.OfType<IDirectSaverFactory>().FirstOrDefault();
                    if (o == null)
                        throw new Exception("Unable to find IDirectSaverFactory in ShamanOptions.Services.");
                    var result = o.GetDirectSaver<T>(GetType(), optionsFactory);
                    _directSaverCache[typeof(T)] = result;
                    return result;
                }
                finally
                {
                    _lock.ExitWriteLock();
                }
            }
            finally
            {
                _lock.ExitUpgradeableReadLock();
            }
        }


        private readonly ReaderWriterLockSlim _lock = new ReaderWriterLockSlim();
        private readonly IDictionary<Type, object> _directSaverCache = new Dictionary<Type, object>();
    }
}