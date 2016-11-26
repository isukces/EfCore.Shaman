#region using

using System;
using EfCore.Shaman.SqlServer;

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
                .With<SqlServerFixerService>();
        }

        #endregion
    }
}