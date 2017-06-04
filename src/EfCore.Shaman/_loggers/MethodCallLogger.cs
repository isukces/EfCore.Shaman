#region using

using System;

#endregion

namespace EfCore.Shaman
{
    public class MethodCallLogger : IShamanLogger
    {
        #region Constructors

        public MethodCallLogger(Action<ShamanLogMessage> action)
        {
            _action = action;
        }

        #endregion

        #region Instance Methods

        public void Log(ShamanLogMessage info)
        {
            try
            {
                _action?.Invoke(info);
            }
            catch { }
        }

        #endregion

        #region Fields

        private readonly Action<ShamanLogMessage> _action;

        #endregion
    }
    
}