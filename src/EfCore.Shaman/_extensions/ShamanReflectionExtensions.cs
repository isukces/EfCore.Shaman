#region using

using System;
using System.Linq;
using System.Reflection;

#endregion

namespace EfCore.Shaman
{
    public static class ShamanReflectionExtensions
    {
        #region Static Methods

        public static MethodInfo FindStaticMethodWihoutParameters(this Type dbContextType, string name)
        {
            const BindingFlags allStatic = BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic;
            if (dbContextType == null) throw new ArgumentNullException(nameof(dbContextType));
#if FXCORE
            var method = dbContextType
                .GetMethods(allStatic)
                .FirstOrDefault(a => 
                    a.Name==name 
                    && a.GetParameters().Length==0);
#else
            var method = dbContextType
                .GetMethod("GetShamanOptions",
                    allStatic,
                    null,
                    new Type[0],
                    null);
#endif
            return method;
        }

        #endregion
    }
}