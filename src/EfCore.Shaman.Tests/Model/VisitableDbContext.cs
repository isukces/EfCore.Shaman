#region using

using System;
using Microsoft.EntityFrameworkCore;

#endregion

namespace EfCore.Shaman.Tests.Model
{
    internal abstract class VisitableDbContext : DbContext
    {
        #region Constructors

        protected VisitableDbContext(DbContextOptions options) : base(options)
        {
        }

        #endregion

        #region Properties

        public Action<ModelBuilder> ExternalCheckModel { get; set; }

        public DbSet<MySettingsEntity> Settings { get; set; }

        #endregion
    }
}