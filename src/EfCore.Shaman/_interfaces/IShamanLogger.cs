#region using

using System;

#endregion

namespace EfCore.Shaman
{
    public interface IShamanLogger
    {
        #region Instance Methods

        void Log(string source, string message);

        #endregion
    }

    public static class ShamanLoggerExtension
    {
        #region Constructors

        public static void Log(this IShamanLogger src, Type t, string method, string message)
        {
            src.Log($"{t.Name}.{method}", message);
        }

        #endregion
    }
}