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
            var alreadyCreatedCatalogs = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            foreach (var dbSetInfo in info.DbSets)
            {
                foreach (var indexInfo in dbSetInfo.Indexes.Where(a => a.IndexType == IndexType.FullTextIndex))
                {
                    log("Fixing full text index " + indexInfo.IndexName);
                    if (string.IsNullOrEmpty(indexInfo.FullTextCatalogName))
                        throw new Exception("FullTextCatalog name not set: full text index on table " + dbSetInfo.TableName);
                    if (alreadyCreatedCatalogs.Add(dbSetInfo.TableName))
                    {
                        var sql = CreateSqlCreateFullTextCatalogIfNotExists(indexInfo.FullTextCatalogName);
                        migrationBuilder.Sql(sql);
                    }
                }
            }
            log("End");
        }

        public static string CreateSqlCreateFullTextCatalogIfNotExists(string name)
            => $"if not exists (SELECT * FROM sys.fulltext_catalogs where name={MsSqlUtils.QuoteText(name)}) CREATE FULLTEXT CATALOG {MsSqlUtils.Escape(name)};";
    }
}
