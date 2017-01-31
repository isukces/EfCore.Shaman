#region using

using System;

#endregion

namespace EfCore.Shaman
{
    public interface IShamanLogger
    {
        #region Instance Methods

        void Log(ShamanLogMessage logInfo);

        #endregion
    }

    public struct ShamanLogMessage
    {
        #region Properties

        public string Source { get; set; }
        public string Message { get; set; }

        #endregion
    }

    public static class ShamanLoggerExtension
    {
        #region Static Methods

        public static IShamanLogger Append(this IShamanLogger src, IShamanLogger another)
        {
            if (src.IsNullOrEmpty())
                return another ?? EmptyShamanLogger.Instance;
            if (another.IsNullOrEmpty())
                return src ?? EmptyShamanLogger.Instance;
            return new MethodCallLogger(info =>
            {
                src.Log(info);
                another.Log(info);
            });
        }

        public static bool IsNullOrEmpty(this IShamanLogger src)
        {
            return src == null || src is EmptyShamanLogger;
        }

        public static void Log(this IShamanLogger src, Type t, string method, string message)
        {
            if (src.IsNullOrEmpty())
                return;
            var info = new ShamanLogMessage
            {
                Source = $"{t.Name}.{method}",
                Message = message
            };
            src.Log(info);
        }

        #endregion
    }
}