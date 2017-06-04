#region using

using EfCore.Shaman.Services;
using Microsoft.EntityFrameworkCore;

#endregion

namespace EfCore.Shaman.Tests.Model
{
    internal class SingularTableNamesDbContext : VisitableDbContext
    {
        public SingularTableNamesDbContext(DbContextOptions options) : base(options)
        {
        }


        public static ShamanOptions GetShamanOptions()
        {
            return ShamanOptions.Default.With(new RemovePluralizingTableNameService())
                .WithLogger(new MethodCallLogger(LogToConsoleWhileMigrationService.LogInfoToConsole));
        }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            this.FixOnModelCreating(modelBuilder);
            ExternalCheckModel?.Invoke(modelBuilder);
        }


        public DbSet<User> Users { get; set; }
    }
}