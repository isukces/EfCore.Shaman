#region using

using System;
using System.Reflection;
using EfCore.Shaman.ModelScanner;

#endregion

namespace EfCore.Shaman.SqlServer
{
    class SqlServerReflectionService : IColumnInfoUpdateService, IDbSetInfoUpdateService
    {
        #region Instance Methods

        public void UpdateColumnInfo(ColumnInfo columnInfo, PropertyInfo propertyInfo)
        {
            var attribute = propertyInfo.GetCustomAttribute<SqlServerCollationAttribute>();
            var collation = attribute?.Collation?.Trim();
            if (string.IsNullOrEmpty(collation)) return;
            columnInfo.Annotations[Ck] = collation;
        }

        #endregion

        #region Other

        public const string Prefix = "SqlServer.";
        public const string Ck = Prefix + "Collation";

        #endregion

        public void UpdateDbSetInfo(DbSetInfo dbSetInfo, Type entityType, Type contextType)
        {
            if (string.IsNullOrEmpty(dbSetInfo.Schema))
                dbSetInfo.Schema = "dbo";
            var attribute = entityType.GetCustomAttribute<SqlServerCollationAttribute>();
            var collation = attribute?.Collation?.Trim();
            if (string.IsNullOrEmpty(collation)) return;
            dbSetInfo.Annotations[Ck] = collation;
        }
    }
}