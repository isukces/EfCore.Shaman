using System;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using EfCore.Shaman.Tests.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Xunit;

namespace EfCore.Shaman.Tests.EfCore.Shaman.Services
{
    public class TimestampAttributeUpdaterTest
    {
        [Fact]
        public void T01_ShouldMarkColumnAsTimestamp()
        {
            ModelInfoTests.DoTestOnModelBuilder<TimestampAttributeUpdaterDbContext>(false, mb =>
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

        public static ShamanOptions GetShamanOptions(Type dbContextType)
        {
            var options = ShamanOptions.GetDefault(dbContextType)
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

            IEntityType et = modelBuilder.Model.GetEntityTypes().First();
            Debug.WriteLine($"ClrType={et.ClrType} from {et.ClrType.GetTypeInfo().Assembly.FullName}");
            Debug.WriteLine($"GetType()={et.GetType()} from {et.GetType().GetTypeInfo().Assembly.FullName}");

            this.FixOnModelCreating(modelBuilder);
            ExternalCheckModel?.Invoke(modelBuilder);
        }

        public DbSet<UserWithTimestamp> Users { get; set; }
    }
}