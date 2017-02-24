#region using

using System;
using System.Collections.Generic;
using EfCore.Shaman.ModelScanner;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;

#endregion

namespace EfCore.Shaman.SqlServer.DirectSql
{
    internal class DeleteBuilder : DirectSqlBase
    {
        #region Constructors

        private DeleteBuilder()
        {
        }

        #endregion

        #region Static Methods

        public static void DoDelete(IFullTableName fullTableName, DbContext context,
            IReadOnlyDictionary<string, object> keys)
        {
            var a = new DeleteBuilder
            {
                FullTableName = fullTableName
            };
            a.Delete(context, keys);
        }

        public static void DeleteByPrimaryKey(IFullTableName fullTableName, DbContext context, ColumnInfo[] sqlColumns,
            ColumnInfo[] pkColumns, object[] keyValues)
        {
            var a = new DeleteBuilder
            {
                FullTableName = fullTableName,
                SqlColumns = sqlColumns
            };
            a.DeleteByPrimaryKey(context, keyValues);
        }

        #endregion

        #region Instance Methods

        private void Delete(DbContext context, IReadOnlyDictionary<string, object> keys)
        {
            PrepareUpdateSql(keys);
            var sql = SqlText.ToString();
            var tmp = context.Database.ExecuteSqlCommand(sql, ParameterValues.ToArray());
        }

        private void DeleteByPrimaryKey(DbContext context, [NotNull] object[] keyValues)
        {
            if (keyValues == null) throw new ArgumentNullException(nameof(keyValues));
            if (PkColumns.Count != keyValues.Length)
                throw new ArgumentNullException($"{nameof(keyValues)} should contain {PkColumns.Count} element(s)");
            var dict = new Dictionary<string, object>();
            for (var i = 0; i < PkColumns.Count; i++)
                dict[PkColumns[i].ColumnName] = keyValues[i];
            Delete(context, dict);
        }

        private void PrepareUpdateSql(IReadOnlyDictionary<string, object> keys)
        {
            SqlText.AppendFormat("delete from {0} ", TableName);
            SqlText.Append(" where ");
            Separator = null;
            foreach (var i in keys)
            {
                AddSeparator(" AND ");
                SqlText.Append(Encode(i.Key) + "=@p" + ParameterValues.Count);
                ParameterValues.Add(i.Value);
            }
        }

        #endregion
    }
}