#region using

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

#endregion

namespace EfCore.Shaman
{
    public sealed class ShamanOptions
    {
        #region Static Methods

        public static ShamanOptions CreateShamanOptions(Type dbContextType)
        {
            if (dbContextType == null) throw new ArgumentNullException(nameof(dbContextType));
#if NETCORE
            var method = dbContextType
                .GetMethods(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic)
                .FirstOrDefault(a => 
                    a.Name=="GetShamanOptions" 
                    && a.GetParameters().Length==0 
                    && a.ReturnType==typeof(ShamanOptions));
#else
            var method = dbContextType
                .GetMethod("GetShamanOptions",
                    BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic,
                    null,
                    new Type[0],
                    null);
#endif
            if (method == null || method.ReturnType != typeof(ShamanOptions))
                return Default;
            return (ShamanOptions)method.Invoke(null, null);
        }

        #endregion

        #region Static Properties

        public static ShamanOptions Default
            => new ShamanOptions().WithDefaultServices();

        #endregion

        #region Properties

        public IList<IShamanService> Services { get; } = new List<IShamanService>();

        #endregion
    }
}