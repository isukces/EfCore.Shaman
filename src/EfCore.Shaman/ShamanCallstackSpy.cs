#region using

using System;
using System.Diagnostics;
using System.Reflection;

#endregion

namespace EfCore.Shaman
{
    public static class ShamanCallstackSpy
    {
        #region Static Methods

        private static string GetDeclaringTypeFullName(MethodBase method)
        {
            try
            {
                return method.DeclaringType?.FullName;
            }
            catch
            {
                return null;
            }
        }

        #endregion

        #region Static Properties

        public static CallerInfoType CallerInfo
        {
            get
            {
#if NET451
                var st = new StackTrace(true);
                for (var i = 0; i < st.FrameCount; i++)
                {
                    var frame = st.GetFrame(i);
                    var method = frame.GetMethod();
                    var declaringTypeFullName = GetDeclaringTypeFullName(method);
                    if (declaringTypeFullName != "Microsoft.EntityFrameworkCore.Design.Internal.MigrationsOperations")
                        continue;
                    switch (method.Name)
                    {
                        case "RemoveMigration":
                            return CallerInfoType.RemoveMigration;
                        case "AddMigration":
                            return CallerInfoType.AddMigration;
                    }
                }
#else
#warning CallerInfo is not supported :(
#endif
                return CallerInfoType.Other;
            }
        }

#endregion
    }

    public enum CallerInfoType
    {
        /// <summary>
        /// Not recognized, usually run outside design tools
        /// </summary>
        Other,

        /// <summary>
        ///     Called from Microsoft.EntityFrameworkCore.Design.Internal.MigrationsOperations.AddMigration
        /// </summary>
        AddMigration,

        /// <summary>
        ///     called from Microsoft.EntityFrameworkCore.Design.Internal.MigrationsOperations.RemoveMigration
        /// </summary>
        RemoveMigration
    }
}