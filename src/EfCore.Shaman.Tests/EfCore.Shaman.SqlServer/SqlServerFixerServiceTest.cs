#region using

using EfCore.Shaman.SqlServer;
using Xunit;

#endregion

namespace EfCore.Shaman.Tests.EfCore.Shaman.SqlServer
{
    public class SqlServerFixerServiceTest
    {
        [Fact]
        public void T01_ShouldCreateChangeCollationSql()
        {
            var sql = SqlServerFixerService.CreateChangeCollationSql(
                "[dbo].[Table]",
                "[Column]",
                KnownCollations.Polish100CiAs,
                true, 123, false);
            var expected = "ALTER TABLE [dbo].[Table] ALTER COLUMN [Column] nvarchar(123) COLLATE Polish_100_CI_AS NOT NULL";
            Assert.Equal(expected, sql);
        }
        
        
    }
}