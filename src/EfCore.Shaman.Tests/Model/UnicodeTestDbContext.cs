using System.ComponentModel.DataAnnotations;
using EfCore.Shaman.Services;
using EfCore.Shaman.SqlServer;
using Microsoft.EntityFrameworkCore;

namespace EfCore.Shaman.Tests.Model
{
    internal class UnicodeTestDbContext : VisitableDbContext
    {
        public UnicodeTestDbContext()
        {
        }

        public UnicodeTestDbContext(DbContextOptions options) : base(options)
        {
        }

        public static ShamanOptions GetShamanOptions()
        {
            return ShamanOptions.GetDefault<UnicodeTestDbContext>()
                .With(new RemovePluralizingTableNameService())
                .WithSqlServer()
                .WithLogger(new MethodCallLogger(
                    LogToConsoleWhileMigrationService.LogInfoToConsole,
                    LogToConsoleWhileMigrationService.LogExceptionToConsole
                ));
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer(".");
            }
        }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            this.FixOnModelCreating(modelBuilder);
            ExternalCheckModel?.Invoke(modelBuilder);
        }


        public DbSet<SomeEntity> MyTestTable { get; set; }


        public class SomeEntity
        {
            public int Id { get; set; }
            [SqlServerCollation(KnownCollations.Croatian100CiAi)]
            public string DefaultNoLength { get; set; }

            [MaxLength(128)]
            [SqlServerCollation(KnownCollations.Croatian100CiAi)]
            public string Default { get; set; }

            [UnicodeText]
            [SqlServerCollation(KnownCollations.Croatian100CiAi)]
            public string UnicodeNoLength { get; set; }

            [MaxLength(128)]
            [UnicodeText]
            [SqlServerCollation(KnownCollations.Croatian100CiAi)]
            public string Unicode { get; set; }


            [UnicodeText(false)]
            public string NoUnicodeNoLength { get; set; }

            [MaxLength(128)]
            [UnicodeText(false)]
            [SqlServerCollation(KnownCollations.Croatian100CiAi)]
            public string NoUnicode { get; set; }
        }
    }
}