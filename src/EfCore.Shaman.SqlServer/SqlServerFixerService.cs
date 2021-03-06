﻿#region using

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
    public sealed class SqlServerFixerService : IFixMigrationUpService, IModelPrepareService
    {
        public static string CreateChangeCollationSql(string escapedTableName, string escapedColumnName,
            string columnCollation, bool? isUnicode, int? maxLength, bool isNullable)
        {
            var qu = new StringBuilder();
            qu.Append("ALTER TABLE ");
            qu.Append(escapedTableName);
            qu.Append(" ALTER COLUMN ");
            qu.Append(escapedColumnName);
            var def = MkStringType(isUnicode == true, maxLength, columnCollation);
            qu.Append(" " + def);
            qu.Append(isNullable ? " NULL" : " NOT NULL");
            return qu.ToString();
        }

        public static string MkStringType(bool isUnicode, int? maxLength, string columnCollation)
        {
            var qu = new StringBuilder();
            qu.Append(isUnicode ? "nvarchar" : " varchar");
            qu.Append($"({MsSqlUtils.GetStringLength(maxLength)})");
            if (!string.IsNullOrEmpty(columnCollation))
                qu.Append(" COLLATE " + columnCollation);
            return qu.ToString();
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

        public void FixMigrationUp(MigrationBuilder migrationBuilder, ModelInfo info)
        {
            if (!MsSqlUtils.IsSupportedProvider(migrationBuilder.ActiveProvider))
                return;
            var logger = info.UsedShamanOptions.Logger ?? EmptyShamanLogger.Instance;
            var log    = logger.CreateMethod(typeof(SqlServerFixerService), nameof(FixMigrationUp));
            log("Begin");
            var createTableOperations = new Dictionary<FullTableName, CreateTableOperation>();
            foreach (var i in migrationBuilder.Operations.OfType<CreateTableOperation>())
            {
                var tn =new FullTableName(i.Name, i.Schema).WithDefaultSchema(info.DefaultSchema);
                createTableOperations[tn] = i;
            }

            foreach (var dbSetInfo in info.DbSets)
            {
                // looking for create operation
                var tn =new FullTableName(dbSetInfo.TableName, dbSetInfo.Schema).WithDefaultSchema(info.DefaultSchema);
                if (!createTableOperations.TryGetValue(tn, out var createTableOperation))
                    continue;
                log($"Fixing create table {tn}");
                // var tableCollation = GetCollation(dbSetInfo.Annotations);
                var columns = createTableOperation.Columns.ToDictionary(a => a.Name, a => a,
                    StringComparer.OrdinalIgnoreCase);
                foreach (var columnInfo in dbSetInfo.Properites.Where(q => !q.IsNotMapped && !q.IsNavigationProperty))
                {
                    if (!columns.TryGetValue(columnInfo.ColumnName, out var addColumnOperation)) continue;
                    if (addColumnOperation.ClrType == typeof(string))
                    {
                        // fix collation and unicode for text field
                        var usedIsUnicode = MsSqlUtils.IsUnicodeTextField(addColumnOperation.ColumnType);
                        var expectedUnicode = addColumnOperation.IsUnicode ?? usedIsUnicode;

                        var columnCollation = SqlServerReflectionService.GetCollation(columnInfo, dbSetInfo, info);
                        if (string.IsNullOrEmpty(columnCollation) && usedIsUnicode == expectedUnicode) continue;
                        var escapedTableName  = MsSqlUtils.Escape(dbSetInfo.Schema, dbSetInfo.TableName);
                        var escapedColumnName = MsSqlUtils.Escape(columnInfo.ColumnName);
                        if (!string.IsNullOrEmpty(columnCollation))
                            log($"Change collation {escapedTableName}.{escapedColumnName} => {columnCollation}");
                        if (usedIsUnicode != expectedUnicode)
                            log($"Change unicode {escapedTableName}.{escapedColumnName} => {expectedUnicode}");
                        var sql = CreateChangeCollationSql(escapedTableName, escapedColumnName, columnCollation,
                            expectedUnicode, addColumnOperation.MaxLength, addColumnOperation.IsNullable);
                        migrationBuilder.Sql(sql);
                        MoveSqlBeforeIndexCreation(migrationBuilder, createTableOperation, columnInfo.ColumnName);
                    }
                }
            }

            log("End");
        }

        public void UpdateModel(IUpdatableSimpleModelInfo simpleModelInfo, Type dbContextType, IShamanLogger logger)
        {
            if (!string.IsNullOrEmpty(simpleModelInfo.DefaultSchema))
                return;
            var log = logger.CreateMethod(typeof(SqlServerFixerService), nameof(UpdateModel));
            simpleModelInfo.DefaultSchema = MsSqlUtils.DefaultSchema;
            log("Change default schema to [" + simpleModelInfo.DefaultSchema + "]");
        }
    }
}