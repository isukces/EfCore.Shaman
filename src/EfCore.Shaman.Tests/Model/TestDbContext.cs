#region using

using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

#endregion

namespace EfCore.Shaman.Tests.Model
{
    [DefaultSchema("testSchema")]
    internal class TestDbContext : VisitableDbContext
    {
        #region Constructors

        public TestDbContext(DbContextOptions options) : base(options)
        {

        }

        #endregion

        #region Static Methods

        private static ShamanOptions GetShamanOptions()
        {
            return ShamanOptions
                .GetDefault(typeof(TestDbContext ))
                .With<EmptyService>();
        }

        #endregion

        #region Instance Methods

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // manual changes 
            IMutableEntityType er = modelBuilder.Model.GetEntityTypes()
                .Single(a => a.ClrType == typeof(MyEntityWithDifferentTableName));
            er.Relational().TableName = "ManualChange";

            this.FixOnModelCreating(modelBuilder);
            ExternalCheckModel?.Invoke(modelBuilder);
        }

        #endregion

        #region Properties

        public DbSet<MyEntityWithUniqueIndex> EntityWithUniqueIndex { get; set; }
        public DbSet<MyEntityWithFullTextIndex> EntityWithFullTextIndex { get; set; }
        public DbSet<MyEntityWithDifferentTableName> EntityWithDifferentTableName { get; set; }

        #endregion
    }
}