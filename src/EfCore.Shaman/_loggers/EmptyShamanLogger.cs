namespace EfCore.Shaman
{
    public sealed class EmptyShamanLogger : IShamanLogger
    {
        public void Log(ShamanLogMessage info)
        {
        }

        public static IShamanLogger Instance => InstanceHolder.MyInstance;

        private static class InstanceHolder
        {
            public static readonly IShamanLogger MyInstance = new EmptyShamanLogger();
        }
    }
}