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
            var log = logger.CreateMethod(GetType(), nameof(FixMigrationUp));
            log("Begin");
            foreach (var dbSetInfo in info.DbSets)
            {
                foreach (var i in dbSetInfo.Indexes.Where(a => a.IndexType == IndexType.FullTextIndex))
                {
                    log("Fixing full text index " + i.IndexName);
                }
                /* var columnCollation = GetCollation(columnInfo.Annotations);
                 if (string.IsNullOrEmpty(columnCollation)) continue;
                 var escapedTableName = MsSqlUtils.Escape(dbSetInfo.Schema, dbSetInfo.TableName);
                 var escapedColumnName = MsSqlUtils.Escape(columnInfo.ColumnName);
                 log($"Change collation {escapedTableName}.{escapedColumnName} => {columnCollation}");
                 // looking for column for operation
                 AddColumnOperation addColumnOperation;
                 if (!columns.TryGetValue(columnInfo.ColumnName, out addColumnOperation)) continue;
                 var qu = new StringBuilder();
                 
                 MoveSqlBeforeIndexCreation(migrationBuilder, createTableOperation, columnInfo.ColumnName);*/
            }
            log("End");
        }

        public static string CreateSqlCreateFullTextCatalogIfNotExists(string name) 
            => $"if not exists (SELECT * FROM sys.fulltext_catalogs where name={MsSqlUtils.QuoteText(name)}) CREATE FULLTEXT CATALOG {MsSqlUtils.Escape(name)};";
    }
}
