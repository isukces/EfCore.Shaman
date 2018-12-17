using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;

namespace EfCore.Shaman
{
    public sealed class ShamanOptions
    {
        public static ShamanOptions CreateShamanOptions(Type dbContextType)
        {
            if (dbContextType == null) throw new ArgumentNullException(nameof(dbContextType));
            var method = dbContextType.FindStaticMethodWihoutParameters("GetShamanOptions");
            if (method == null || method.ReturnType != typeof(ShamanOptions))
                return GetDefault(dbContextType);
            return (ShamanOptions)method.Invoke(null, null);
        }

        [CanBeNull]
        public static IShamanLogger TryGetExceptionLogger(Type dbContextType)
        {
            try
            {
                return CreateShamanOptions(dbContextType)?.Logger;
            }
            catch
            {
                return null;
            }
        }

        public static ShamanOptions GetDefault(Type dbContextType)
        {
            return new ShamanOptions().WithDefaultServices(dbContextType);
        }

        public static ShamanOptions GetDefault<T>()
            where T:DbContext
        {
            return GetDefault(typeof(T));
        }

        public IList<IShamanService> Services { get; } = new List<IShamanService>();

        public IShamanLogger Logger { get; set; } = EmptyShamanLogger.Instance;
    }
}