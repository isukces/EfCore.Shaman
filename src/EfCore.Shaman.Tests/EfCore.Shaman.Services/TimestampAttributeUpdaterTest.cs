using System.Linq;
using EfCore.Shaman.Tests.Model;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace EfCore.Shaman.Tests.EfCore.Shaman.Services
{
    public class TimestampAttributeUpdaterTest
    {
        [Fact]
        public void T01_ShouldMarkColumnAsTimestamp()
        {
            ModelInfoTests.DoTestOnModelBuilder<TimestampAttributeUpdaterDbContext>(mb =>
            {
                var t = mb.Model.GetEntityTypes().Single(a => a.ClrType == typeof(UserWithTimestamp));
                Assert.NotNull(t);

                var modelInfo = ModelInfoTests.GetModelInfo<TimestampAttributeUpdaterDbContext>();
                var dbSet = modelInfo.DbSet<UserWithTimestamp>();
                Assert.NotNull(dbSet);
                var c = dbSet.Properites.Single(a => a.IsTimestamp);
                Assert.Equal("RowVersion", c.ColumnName);
            }, ctx =>
            {
                var b = ctx.GetDirectSaver<UserWithTimestamp>();
                // b.Insert(ctx, new UserWithTimestamp());
            });
        }
    }

    internal class TimestampAttributeUpdaterDbContext : VisitableDbContext
    {
        public TimestampAttributeUpdaterDbContext(DbContextOptions options) : base(options)
        {
        }

        public static ShamanOptions GetShamanOptions()
        {
            var options = ShamanOptions.Default
                // .WithLogger(new MethodCallLogger(WriteDebugInfo))
                .WithSqlServer();
            //.With<LogToConsoleWhileMigrationService>()
            //.With<SqlDbContextValueProvider>();
            // return ShamanOptions.Default.With(new PrefixedTableNameService("myPrefix"));
            return options;
        }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            this.FixOnModelCreating(modelBuilder);
            ExternalCheckModel?.Invoke(modelBuilder);
        }

        public DbSet<UserWithTimestamp> Users { get; set; }
    }
}