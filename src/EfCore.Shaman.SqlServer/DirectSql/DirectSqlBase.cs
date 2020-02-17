#region using

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EfCore.Shaman.ModelScanner;
using Microsoft.EntityFrameworkCore;

#endregion

namespace EfCore.Shaman.SqlServer.DirectSql
{
    internal class DirectSqlBase
    {
        protected DirectSqlBase()
        {
            _pkColumns = new Lazy<IReadOnlyList<ColumnInfo>>(GetPkColumnsInternal);
        }

        public static string Encode(string x)
        {
            return "[" + x + "]";
        }

        protected static async Task ExecSqlAndUpdateBackAsync(string sqlText, DbContext context, object entity,
            IReadOnlyList<ColumnInfo> returned, object[] parameterValues)
        {
            if (returned != null && returned.Any())
            {
                using(var reader = await context.Database.ExecuteReaderAsync(sqlText, parameterValues))
                {
                    while (reader.DbDataReader.Read())
                    {
                        var v = new object[reader.DbDataReader.FieldCount];
                        reader.DbDataReader.GetValues(v);
                        for (var index = 0; index < returned.Count; index++)
                            returned[index].ValueWriter.WritePropertyValue(entity, v[index]);
                        break;
                    }
                }
            }
            else
            {
                var tmp = context.Database.ExecuteSqlCommand(sqlText, parameterValues);
            }
        }

        protected void AddSeparator(string newSeparator)
        {
            if (Separator == null)
                Separator = newSeparator;
            else
                SqlText.Append(Separator);
        }

        private IReadOnlyList<ColumnInfo> GetPkColumnsInternal()
        {
            var columnInfos = new List<ColumnInfo>(SqlColumns.Count);
            foreach (var i in SqlColumns)
                if (i.IsInPrimaryKey)
                    columnInfos.Add(i);
            return columnInfos.ToArray();
        }

        protected IReadOnlyList<ColumnInfo> PkColumns => _pkColumns.Value;

        protected string TableName => Encode(FullTableName.Schema) + "." + Encode(FullTableName.TableName);

        protected readonly StringBuilder SqlText = new StringBuilder();

        protected readonly List<object> ParameterValues = new List<object>();

        protected string Separator;

        protected IFullTableName FullTableName;

        protected ColumnInfo IdentityColumn;

        protected IReadOnlyList<ColumnInfo> SqlColumns;

        private readonly Lazy<IReadOnlyList<ColumnInfo>> _pkColumns;

        protected object Entity;
    }
}