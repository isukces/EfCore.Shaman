using System.Collections.Generic;
using System.Linq;
using EfCore.Shaman.ModelScanner;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;

namespace EfCore.Shaman.SqlServer.DirectSql
{
    internal class UpdateBuilder : DirectSqlBase
    {
        private UpdateBuilder()
        {
        }


        public static void DoUpdate(IFullTableName fullTableName, DbContext context, ColumnInfo[] sqlColumns,
            ColumnInfo identityColumn, object entity, bool skipSelect = false)
        {
            var a = new UpdateBuilder
            {
                FullTableName = fullTableName,
                SqlColumns = sqlColumns,
                Entity = entity,
                IdentityColumn = identityColumn
            };
            a.Update(context, skipSelect);
        }

        [NotNull]
        private IReadOnlyList<ColumnInfo> PrepareUpdateSql(bool skipSelect)
        {
            var whereSql = PrepareWhereSql();
            SqlText.AppendFormat("Update {0} set ", TableName);
            Separator = null;
            for (var index = 0; index < SqlColumns.Count; index++)
            {
                var columnInfo = SqlColumns[index];
                if (columnInfo.IsInPrimaryKey || columnInfo.IsDatabaseGenerated)
                    continue;
                AddSeparator(", ");
                SqlText.Append(Encode(columnInfo.ColumnName) + "=@p" + ParameterValues.Count);
                ParameterValues.Add(columnInfo.ValueReader.ReadPropertyValue(Entity));
            }
            SqlText.Append(whereSql);
            if (skipSelect)
                return new ColumnInfo[0];
            var returned = SqlColumns.Where(a => a.IsDatabaseGenerated && !a.IsInPrimaryKey).ToList();
            if (!returned.Any()) return returned;
            SqlText.Append("; select " + string.Join(",", returned.Select(q => Encode(q.ColumnName))));
            SqlText.Append(" from " + TableName);
            SqlText.Append(whereSql);
            return returned;
        }

        private string PrepareWhereSql()
        {
            SqlText.Append(" where ");
            Separator = null;
            foreach (var i in SqlColumns)
            {
                if (!i.IsInPrimaryKey)
                    continue;
                AddSeparator(" AND ");
                SqlText.Append(Encode(i.ColumnName) + "=@p" + ParameterValues.Count);
                ParameterValues.Add(i.ValueReader.ReadPropertyValue(Entity));
            }
            var whereSql = SqlText.ToString();
            SqlText.Clear();
            return whereSql;
        }


        private void Update(DbContext context, bool skipSelect)
        {
            var returned = PrepareUpdateSql(skipSelect);
            var sql = SqlText.ToString();
            ExecSqlAndUpdateBack(sql, context, Entity, returned, ParameterValues.ToArray());
           
        }
    }
}