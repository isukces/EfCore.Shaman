using System;
using JetBrains.Annotations;

namespace EfCore.Shaman
{
    public class MethodCallLogger : IShamanLogger
    {
        public MethodCallLogger([CanBeNull] Action<ShamanLogMessage> logAction, [CanBeNull] Action<Guid, Exception> exceptionAction)
        {
            _logAction = logAction;
            _exceptionAction = exceptionAction;
        }

        public void Log(ShamanLogMessage info)
        {
            try
            {
                _logAction?.Invoke(info);
            }
            catch { } // no exception logginng
        }

        public void LogException(Guid locationId, Exception exception)
        {
            try
            {
                _exceptionAction?.Invoke(locationId, exception);
            }
            catch { } // no exception logginng
        }

        private readonly Action<ShamanLogMessage> _logAction;
        private readonly Action<Guid, Exception> _exceptionAction;
    }
    
}