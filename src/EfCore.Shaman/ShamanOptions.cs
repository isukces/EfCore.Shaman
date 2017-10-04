using System;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace EfCore.Shaman
{
    public sealed class ShamanOptions
    {
        public static ShamanOptions CreateShamanOptions(Type dbContextType)
        {
            if (dbContextType == null) throw new ArgumentNullException(nameof(dbContextType));
            var method = dbContextType.FindStaticMethodWihoutParameters("GetShamanOptions");
            if (method == null || method.ReturnType != typeof(ShamanOptions))
                return Default;
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

        public static ShamanOptions Default
        {
            get { return new ShamanOptions().WithDefaultServices(); }
        }

        public IList<IShamanService> Services { get; } = new List<IShamanService>();

        public IShamanLogger Logger { get; set; } = EmptyShamanLogger.Instance;
    }
}