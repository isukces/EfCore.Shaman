#region using

using System;

#endregion

namespace EfCore.Shaman
{
#if net45
    [Serializable]
#endif
    public class EmptyConnectionStringException : Exception
    {
        #region Constructors

        public EmptyConnectionStringException() : base(DefaultMessage)
        {
        }

        public EmptyConnectionStringException(string message) : base(message ?? DefaultMessage)
        {
        }

        #endregion

        #region Static Methods

        public static void Check(string connectionString, string message = null)
        {
            if (string.IsNullOrEmpty(connectionString))
                throw new EmptyConnectionStringException(message);
        }

        #endregion

        #region Other

        private const string DefaultMessage = "Connection string can't be empty";

        #endregion
    }
}