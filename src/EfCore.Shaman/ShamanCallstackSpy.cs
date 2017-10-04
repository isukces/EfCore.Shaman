using System;
using System.Diagnostics;
using System.Reflection;

namespace EfCore.Shaman
{
    public static class ShamanCallstackSpy
    {
        private static string GetDeclaringTypeFullName(MethodBase method, IShamanLogger logger)
        {
            try
            {
                return method.DeclaringType?.FullName;
            }
            catch (Exception e)
            {
                logger?.LogException(Guid.Parse("{CE9439CA-5A1C-4ABC-9587-926F91B3AD22}"), e);
                return null;
            }
        }

        public static CallerInfoType GetCallerInfo(IShamanLogger logger)
        {
#if NET451
            var st = new StackTrace(true);
            for (var i = 0; i < st.FrameCount; i++)
            {
                var frame = st.GetFrame(i);
                var method = frame.GetMethod();
                var declaringTypeFullName = GetDeclaringTypeFullName(method, logger);
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