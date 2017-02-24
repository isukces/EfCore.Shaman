#region using

using System.Collections.Generic;
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

        #endregion

        #region Instance Methods

        private void Delete(DbContext context, IReadOnlyDictionary<string, object> keys)
        {
            PrepareUpdateSql(keys);
            var sql = SqlText.ToString();
            var tmp = context.Database.ExecuteSqlCommand(sql, ParameterValues.ToArray());
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