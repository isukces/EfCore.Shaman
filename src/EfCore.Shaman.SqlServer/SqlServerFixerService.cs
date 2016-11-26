#region using

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EfCore.Shaman.ModelScanner;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Migrations.Operations;

#endregion

namespace EfCore.Shaman.SqlServer
{
    internal class SqlServerFixerService : IFixMigrationUpService
    {
        #region Static Methods

        private static string GetCollation(IDictionary<string, object> iAnnotations)
        {
            object value;
            iAnnotations.TryGetValue(SqlServerReflectionService.Ck, out value);
            return value as string;
        }

        private static bool IsSupportedProvider(string provider)
        {
            return string.Equals(provider, "Microsoft.EntityFrameworkCore.SqlServer", StringComparison.OrdinalIgnoreCase);
        }

        private static void MoveSqlBeforeIndexCreation(MigrationBuilder migrationBuilder,
            CreateTableOperation createTableOperation, string columnName)
        {
            var startIdx = migrationBuilder.Operations.IndexOf(createTableOperation);
            for (var i = startIdx + 1; i < migrationBuilder.Operations.Count; i++)
            {
                var op = migrationBuilder.Operations[i] as CreateIndexOperation;
                if (op == null) continue;
                // todo: compare schema
                if (!string.Equals(op.Table, createTableOperation.Name, StringComparison.OrdinalIgnoreCase))
                    continue;
                if (!op.Columns.Contains(columnName, StringComparer.OrdinalIgnoreCase)) continue;
                var oo = migrationBuilder.Operations[migrationBuilder.Operations.Count - 1];
                migrationBuilder.Operations.RemoveAt(migrationBuilder.Operations.Count - 1);
                migrationBuilder.Operations.Insert(i, oo);
                break;
            }
        }

        #endregion

        #region Instance Methods

        public void FixMigrationUp(MigrationBuilder migrationBuilder, ModelInfo info)
        {
            if (!IsSupportedProvider(migrationBuilder.ActiveProvider))
                return;

            var createTableOperations = new Dictionary<string, CreateTableOperation>(StringComparer.OrdinalIgnoreCase);
            foreach (var i in migrationBuilder.Operations.OfType<CreateTableOperation>())
                createTableOperations[i.Name] = i;

            foreach (var dbSetInfo in info.DbSets)
            {
                // looking for create operation
                CreateTableOperation createTableOperation;
                if (!createTableOperations.TryGetValue(dbSetInfo.TableName, out createTableOperation))
                    continue;

                var tableCollation = GetCollation(dbSetInfo.Annotations);
                var columns = createTableOperation.Columns.ToDictionary(a => a.Name, a => a,
                    StringComparer.OrdinalIgnoreCase);
                foreach (var columnInfo in dbSetInfo.Properites.Where(a => !a.IsNotMapped))
                {
                    var columnCollation = GetCollation(columnInfo.Annotations);
                    if (string.IsNullOrEmpty(columnCollation)) continue;
                    // looking for column for operation
                    AddColumnOperation addColumnOperation;
                    if (!columns.TryGetValue(columnInfo.ColumnName, out addColumnOperation)) continue;
                    var qu = new StringBuilder();
                    qu.Append("ALTER TABLE ");
                    qu.Append(MsSqlUtils.Escape(dbSetInfo.Schema, dbSetInfo.TableName));
                    qu.Append(" ALTER COLUMN ");
                    qu.Append(MsSqlUtils.Escape(columnInfo.ColumnName));
                    qu.Append(addColumnOperation.IsUnicode == true ? " nvarchar" : " varchar");
                    qu.Append("(" + MsSqlUtils.GetStringLength(addColumnOperation.MaxLength) + ")");
                    qu.Append(" COLLATE " + columnCollation);
                    qu.Append(addColumnOperation.IsNullable ? " NULL" : " NOT NULL");
                    migrationBuilder.Sql(qu.ToString());
                    MoveSqlBeforeIndexCreation(migrationBuilder, createTableOperation, columnInfo.ColumnName);
                }
            }
        }

        #endregion
    }
}