#region using

using System;
using System.Collections.Generic;
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
            var method = dbContextType
                .GetMethod("GetShamanOptions",
                    BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic,
                    null,
                    new Type[0],
                    null);

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