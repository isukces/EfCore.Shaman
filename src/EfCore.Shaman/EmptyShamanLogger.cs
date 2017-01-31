namespace EfCore.Shaman
{
    public class EmptyShamanLogger : IShamanLogger
    {
        #region Instance Methods

        public void Log(string source, string message)
        {
        }

        #endregion

        #region Static Properties

        public static IShamanLogger Instance => InstanceHolder.MyInstance;

        #endregion

        #region Nested

        private static class InstanceHolder
        {
            #region Static Fields

            public static readonly IShamanLogger MyInstance = new EmptyShamanLogger();

            #endregion
        }

        #endregion
    }
}