#region using

using EfCore.Shaman.SqlServer;
using Xunit;

#endregion

namespace EfCore.Shaman.Tests.EfCore.Shaman.SqlServer
{
    public class SqlServerFullTextIndexServiceTest
    {
        #region Instance Methods

        [Fact]
        public void T01_ShouldCreateSqlCreateFullTextCatalogIfNotExists()
        {
            var sql = SqlServerFullTextIndexService.CreateSqlCreateFullTextCatalogIfNotExists("my catalog name");
            const string expected =
                "if not exists (SELECT * FROM sys.fulltext_catalogs where name='my catalog name') CREATE FULLTEXT CATALOG [my catalog name];";
            Assert.Equal(expected, sql);
        }

        #endregion
    }
}