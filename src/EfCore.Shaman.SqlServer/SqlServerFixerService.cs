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
    public sealed class SqlServerFixerService : IFixMigrationUpService
    {
        #region Static Methods

        public static string CreateChangeCollationSql(string escapedTableName, string escapedColumnName,
            string columnCollation, bool? isUnicode, int? maxLength, bool isNullable)
        {
            var qu = new StringBuilder();
            qu.Append("ALTER TABLE ");
            qu.Append(escapedTableName);
            qu.Append(" ALTER COLUMN ");
            qu.Append(escapedColumnName);
            qu.Append(isUnicode == true ? " nvarchar" : " varchar");
            qu.Append($"({MsSqlUtils.GetStringLength(maxLength)})");
            qu.Append(" COLLATE " + columnCollation);
            qu.Append(isNullable ? " NULL" : " NOT NULL");
            return qu.ToString();
        }

        private static string GetCollation(IDictionary<string, object> iAnnotations)
        {
            object value;
            iAnnotations.TryGetValue(SqlServerReflectionService.Ck, out value);
            return value as string;
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
            if (!MsSqlUtils.IsSupportedProvider(migrationBuilder.ActiveProvider))
                return;
            var logger = info.UsedShamanOptions.Logger ?? EmptyShamanLogger.Instance;
            var log = logger.CreateMethod(typeof(SqlServerFixerService), nameof(FixMigrationUp));
            log("Begin");
            var createTableOperations = new Dictionary<string, CreateTableOperation>(StringComparer.OrdinalIgnoreCase);
            foreach (var i in migrationBuilder.Operations.OfType<CreateTableOperation>())
                createTableOperations[i.Name] = i;
            foreach (var dbSetInfo in info.DbSets)
            {
                // looking for create operation
                CreateTableOperation createTableOperation;
                if (!createTableOperations.TryGetValue(dbSetInfo.TableName, out createTableOperation))
                    continue;
                log($"Fixing create table {dbSetInfo.TableName}");
                // var tableCollation = GetCollation(dbSetInfo.Annotations);
                var columns = createTableOperation.Columns.ToDictionary(a => a.Name, a => a,
                    StringComparer.OrdinalIgnoreCase);
                foreach (var columnInfo in dbSetInfo.Properites.Where(q => !q.IsNotMapped && !q.IsNavigationProperty))
                {
                    if (!columns.TryGetValue(columnInfo.ColumnName, out var addColumnOperation)) continue;
                    var usedIsUnicode = addColumnOperation.ColumnType.ToLower().StartsWith("nvarchar");
                    var expectedUnicode = addColumnOperation.IsUnicode ?? usedIsUnicode;

                    var columnCollation = GetCollation(columnInfo.Annotations);
                    if (string.IsNullOrEmpty(columnCollation) && usedIsUnicode == expectedUnicode) continue;
                    var escapedTableName = MsSqlUtils.Escape(dbSetInfo.Schema, dbSetInfo.TableName);
                    var escapedColumnName = MsSqlUtils.Escape(columnInfo.ColumnName);
                    if (!string.IsNullOrEmpty(columnCollation))
                        log($"Change collation {escapedTableName}.{escapedColumnName} => {columnCollation}");
                    if (usedIsUnicode!=expectedUnicode)
                        log($"Change unicode {escapedTableName}.{escapedColumnName} => {expectedUnicode}");
                    var sql = CreateChangeCollationSql(escapedTableName, escapedColumnName, columnCollation,
                        expectedUnicode, addColumnOperation.MaxLength, addColumnOperation.IsNullable);
                    migrationBuilder.Sql(sql);
                    MoveSqlBeforeIndexCreation(migrationBuilder, createTableOperation, columnInfo.ColumnName);
                }
            }
            log("End");
        }

        #endregion
    }
}