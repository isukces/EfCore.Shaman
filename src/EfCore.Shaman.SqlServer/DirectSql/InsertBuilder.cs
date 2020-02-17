using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EfCore.Shaman.ModelScanner;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;

namespace EfCore.Shaman.SqlServer.DirectSql
{
    internal class InsertBuilder : DirectSqlBase
    {
        private InsertBuilder()
        {
        }

        public static Task DoInsertAsync(IFullTableName tableName, DbContext context, ColumnInfo[] sqlColumns,
            ColumnInfo identityColumn, object entity, bool skipSelect = false)
        {
            var builder = new InsertBuilder
            {
                FullTableName = tableName,
                Entity = entity,
                SqlColumns = sqlColumns,
                IdentityColumn = identityColumn
            };
            return builder.InsertAsync(context, skipSelect);
        }

        private string AddPropertyValue(int idx)
        {
            if (!string.IsNullOrEmpty(_parameters[idx]))
                return _parameters[idx];
            _parameters[idx] = "@p" + ParameterValues.Count;
            var col = SqlColumns[idx];
            ParameterValues.Add(col.ValueReader.ReadPropertyValue(Entity));
            return _parameters[idx];
        }

        private async Task InsertAsync(DbContext context, bool skipSelect)
        {
            _parameters = new string[SqlColumns.Count];
            // var ts = DateTime.UtcNow;
            SqlText.AppendLine("SET NOCOUNT ON;");
            PrepareInsertSql();
            var returned = PrepareSelectSql(skipSelect);
            await ExecSqlAndUpdateBackAsync(SqlText.ToString(), context, Entity, returned, ParameterValues.ToArray());
            //Debug.WriteLine("  sql elapsed " + (DateTime.UtcNow - ts).TotalMilliseconds);
            /*

                string sql = @"SET NOCOUNT ON;
    INSERT INTO [RecipeNodes] ([ArticleId], [EnrichedArticleId], [IsActive], [IsDefault], [MigrationSourceId], [Name])
    VALUES (@p0, @p1, @p2, @p3, @p4, @p5);
    SELECT [Id]
    FROM [RecipeNodes]
    WHERE @@ROWCOUNT = 1 AND [Id] = scope_identity();";
    */
        }


        private void PrepareInsertSql()
        {
            SqlText.AppendFormat("INSERT INTO {0} (", TableName);
            Separator = null;
            for (var index = 0; index < SqlColumns.Count; index++)
            {
                var col = SqlColumns[index];
                if (col.IsIdentity || col.IsDatabaseGenerated) continue;
                AddSeparator(", ");
                SqlText.Append(Encode(col.ColumnName));
                AddPropertyValue(index);
            }
            SqlText.Append(") values (");
            Separator = null;
            for (var parameterIdx = 0; parameterIdx < ParameterValues.Count; parameterIdx++)
            {
                AddSeparator(", ");
                SqlText.Append($"@p{parameterIdx}");
            }
            SqlText.AppendLine(");");
        }

        [NotNull]
        private IReadOnlyList<ColumnInfo> PrepareSelectSql(bool skipSelect)
        {
            if (skipSelect)
                return new ColumnInfo[0];
            var returned = SqlColumns.Where(a => a.IsDatabaseGenerated).ToList();
            if (!returned.Any())
                return new ColumnInfo[0];
            Separator = " AND ";
            SqlText.Append("select " + string.Join(",", returned.Select(q => Encode(q.ColumnName))));
            SqlText.Append(" from " + TableName + " WHERE @@ROWCOUNT=1");
            if (IdentityColumn != null)
            {
                AddSeparator(Separator);
                SqlText.Append(Encode(IdentityColumn.ColumnName) + "=scope_identity();");
            }
            else
            {
                for (var index = 0; index < SqlColumns.Count; index++)
                {
                    var col = SqlColumns[index];
                    if (!col.IsInPrimaryKey) continue;
                    var pn = AddPropertyValue(index);
                    AddSeparator(Separator);
                    SqlText.Append(Encode(col.ColumnName) + "=" + pn);
                }
            }
            return returned;
        }

        private string[] _parameters;
    }
}