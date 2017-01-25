#region using

using EfCore.Shaman.Services;
using Microsoft.EntityFrameworkCore;

#endregion

namespace EfCore.Shaman.Tests.Model
{
    internal class PrefixedTableNamesDbContext : VisitableDbContext
    {
        #region Constructors

        public PrefixedTableNamesDbContext(DbContextOptions options) : base(options)
        {
        }

        #endregion

        #region Static Methods

        public static ShamanOptions GetShamanOptions()
        {
            return ShamanOptions.Default.With(new PrefixedTableNameService("myPrefix"));
        }

        #endregion

        #region Instance Methods

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            this.FixOnModelCreating(modelBuilder);
            ExternalCheckModel?.Invoke(modelBuilder);
        }

        #endregion

        #region Properties

        public DbSet<User> Users { get; set; }

        #endregion
    }
}