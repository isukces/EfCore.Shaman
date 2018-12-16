using System;

namespace EfCore.Shaman
{
    public interface IShamanLogger
    {
        void Log(ShamanLogMessage logInfo);
        void LogException(Guid locationId, Exception exception);
    }

    public struct ShamanLogMessage
    {
        public string Source  { get; set; }
        public string Message { get; set; }
    }

    public static class ShamanLoggerExtension
    {
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
            }, (locationId, exception) =>
            {
                src.LogException(locationId, exception);
                another.LogException(locationId, exception);
            });
        }


        public static Action<string> CreateMethod(this IShamanLogger logger, Type type, string method)
        {
            return txt => logger.Log(type, method, txt);
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
                Source  = $"{t.Name}.{method}",
                Message = message
            };
            src.Log(info);
        }

        public static void LogFix(this IShamanLogger logger, string source, Type entityType, string action)
        {
            var logMessage = $"calling {action} for {entityType.Name}";
            var info = new ShamanLogMessage
            {
                Source  = source,
                Message = logMessage
            };
            logger.Log(info);
        }
    }
}