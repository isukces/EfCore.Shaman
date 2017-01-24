#region using

using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;

#endregion

namespace EfCore.Shaman.Tests.Model
{
    internal class TestDbContext : DbContext, IShamanFriendlyDbContext
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
                .Default
                .With<EmptyService>();
        }

        #endregion

        #region Instance Methods

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // manual changes 
            var er = modelBuilder.Model.GetEntityTypes()
                .Single(a => a.ClrType == typeof(MyEntityWithDifferentTableName));
            er.Relational().TableName = "ManualChange";
            if (modelBuilder.FixOnModelCreating(this))
                ExternalCheckModel?.Invoke(modelBuilder);
        }

        #endregion

        #region Properties

        DbContextCreationMode IShamanFriendlyDbContext.CreationMode { get; set; }

        public Action<ModelBuilder> ExternalCheckModel { get; set; }

        public DbSet<MyEntityWithUniqueIndex> EntityWithUniqueIndex { get; set; }
        public DbSet<MyEntityWithDifferentTableName> EntityWithDifferentTableName { get; set; }

        #endregion
    }
}