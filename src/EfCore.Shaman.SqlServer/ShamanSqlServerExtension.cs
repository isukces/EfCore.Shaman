#region using

using System;
using EfCore.Shaman.SqlServer;
using EfCore.Shaman.SqlServer.DirectSql;

#endregion

// ReSharper disable once CheckNamespace

namespace EfCore.Shaman
{
    public static class ShamanSqlServerExtension
    {
        #region Static Methods

        public static ShamanOptions WithSqlServer(this ShamanOptions options)
        {
            return options
                .With<SqlServerReflectionService>()
                .With<SqlServerDirectSaverFactoryService>()
                .With<SqlServerFixerService>()
                .With<SqlServerFullTextIndexService>();
        }

        #endregion
    }
}