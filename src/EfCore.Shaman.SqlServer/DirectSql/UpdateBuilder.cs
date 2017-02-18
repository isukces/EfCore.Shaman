using EfCore.Shaman.ModelScanner;
using Microsoft.EntityFrameworkCore;

namespace EfCore.Shaman.SqlServer.DirectSql
{
    internal class UpdateBuilder : DirectSqlBase
    {
        #region Constructors

        private UpdateBuilder()
        {
        }

        #endregion

        #region Static Methods

        public static void DoUpdate(IFullTableName fullTableName, DbContext context, ColumnInfo[] sqlColumns,
            ColumnInfo identityColumn, object entity)
        {
            var a = new UpdateBuilder
            {
                FullTableName = fullTableName,
                SqlColumns = sqlColumns,
                Entity = entity,
                IdentityColumn = identityColumn
            };
            a.Update(context);
        }

        #endregion

        #region Instance Methods

        private void PrepareUpdateSql()
        {
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
        }


        private void Update(DbContext context)
        {
            // _sb.AppendLine("SET NOCOUNT ON;");
            PrepareUpdateSql();
            var sql = SqlText.ToString();
            var tmp = context.Database.ExecuteSqlCommand(sql, ParameterValues.ToArray());
        }

        #endregion

     
    }
}