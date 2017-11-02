#region using

using System;
using Microsoft.EntityFrameworkCore;

#endregion

namespace EfCore.Shaman.Tests.Model
{
    internal abstract class VisitableDbContext : ShamanDbContext
    {
        protected VisitableDbContext(DbContextOptions options) : base(options)
        {
        }

        protected VisitableDbContext()
        {
        }

        #region Properties

        public Action<ModelBuilder> ExternalCheckModel { get; set; }

        public DbSet<MySettingsEntity> Settings { get; set; }

        #endregion
    }
}