using System;

namespace EfCore.Shaman
{
    public sealed class EmptyShamanLogger : IShamanLogger
    {
        public void Log(ShamanLogMessage info)
        {
        }

        public void LogException(Guid locationId, Exception exception)
        {            
        }

        public static IShamanLogger Instance => InstanceHolder.MyInstance;

        private static class InstanceHolder
        {
            public static readonly IShamanLogger MyInstance = new EmptyShamanLogger();
        }
    }
}