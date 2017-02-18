using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EfCore.Shaman.ModelScanner;
using Microsoft.EntityFrameworkCore;

namespace EfCore.Shaman.SqlServer.DirectSql
{
    internal class InsertBuilder : DirectSqlBase
    {
        #region Constructors

        private InsertBuilder()
        {
        }

        #endregion

        #region Static Methods

        public static void DoInsert(IFullTableName tableName, DbContext context, ColumnInfo[] sqlColumns,
            ColumnInfo identityColumn, object entity)
        {
            var builder = new InsertBuilder
            {
                FullTableName = tableName,
                Entity = entity,
                SqlColumns = sqlColumns,
                IdentityColumn = identityColumn
            };
            builder.Insert(context);
        }

        #endregion

        #region Instance Methods

        private string AddPropertyValue(int idx)
        {
            if (!string.IsNullOrEmpty(_parameters[idx]))
                return _parameters[idx];
            _parameters[idx] = "@p" + ParameterValues.Count;
            var col = SqlColumns[idx];
            ParameterValues.Add(col.ValueReader.ReadPropertyValue(Entity));
            return _parameters[idx];
        }

        private void Insert(DbContext context)
        {
            _parameters = new string[SqlColumns.Count];
            // var ts = DateTime.UtcNow;
            SqlText.AppendLine("SET NOCOUNT ON;");
            PrepareInsertSql();
            var returned = PrepareSelectSql();
            if (returned != null)
            {
                using (var reader = context.Database.ExecuteReader(SqlText.ToString(), ParameterValues.ToArray()))
                {
                    while (reader.DbDataReader.Read())
                    {
                        var v = new object[reader.DbDataReader.FieldCount];
                        reader.DbDataReader.GetValues(v);
                        for (var index = 0; index < returned.Count; index++)
                            returned[index].ValueWriter.WritePropertyValue(Entity, v[index]);
                        break;
                    }
                }
            }
            else
            {
                var tmp = context.Database.ExecuteSqlCommand(SqlText.ToString(), ParameterValues.ToArray());
            }
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
                if (col.IsIdentity) continue;
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

        private List<ColumnInfo> PrepareSelectSql()
        {
            var returned = SqlColumns.Where(a => a.IsDatabaseGenerated).ToList();
            if (!returned.Any())
                return null;
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

        #endregion

        #region Fields

        private string[] _parameters;

        #endregion
    }
}
