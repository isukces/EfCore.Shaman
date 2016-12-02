#region using

using System;

#endregion

namespace EfCore.Shaman.SqlServer.DirectSql
{
    internal class SqlServerDirectSaverFactoryService : IDirectSaverFactory, IShamanService
    {
        #region Instance Methods

        public IDirectSaver<T> GetDirectSaver<T>(Type contextType)
        {
            return SqlServerDirectSaver<T>.FromContext(contextType);
        }

        #endregion
    }
}