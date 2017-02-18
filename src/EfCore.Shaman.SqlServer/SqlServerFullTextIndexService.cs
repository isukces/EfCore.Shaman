using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EfCore.Shaman.ModelScanner;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Migrations.Operations;

namespace EfCore.Shaman.SqlServer
{
    public sealed class SqlServerFullTextIndexService : IFixMigrationUpService
    {

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
                   /* var columnCollation = GetCollation(columnInfo.Annotations);
                    if (string.IsNullOrEmpty(columnCollation)) continue;
                    var escapedTableName = MsSqlUtils.Escape(dbSetInfo.Schema, dbSetInfo.TableName);
                    var escapedColumnName = MsSqlUtils.Escape(columnInfo.ColumnName);
                    log($"Change collation {escapedTableName}.{escapedColumnName} => {columnCollation}");
                    // looking for column for operation
                    AddColumnOperation addColumnOperation;
                    if (!columns.TryGetValue(columnInfo.ColumnName, out addColumnOperation)) continue;
                    var qu = new StringBuilder();
                    qu.Append("ALTER TABLE ");
                    qu.Append(escapedTableName);
                    qu.Append(" ALTER COLUMN ");
                    qu.Append(escapedColumnName);
                    qu.Append(addColumnOperation.IsUnicode == true ? " nvarchar" : " varchar");
                    qu.Append($"({MsSqlUtils.GetStringLength(addColumnOperation.MaxLength)})");
                    qu.Append(" COLLATE " + columnCollation);
                    qu.Append(addColumnOperation.IsNullable ? " NULL" : " NOT NULL");
                    migrationBuilder.Sql(qu.ToString());
                    MoveSqlBeforeIndexCreation(migrationBuilder, createTableOperation, columnInfo.ColumnName);*/
                }
            }
            log("End");
        }

    }
}
