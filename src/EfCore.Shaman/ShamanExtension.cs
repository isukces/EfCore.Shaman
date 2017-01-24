#region using

using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;

#endregion

namespace EfCore.Shaman
{
    public static class ShamanExtension
    {
        #region Static Methods

        public static void FixMigrationUp<T>(this MigrationBuilder migrationBuilder, ShamanOptions shamanOptions = null) where T : DbContext
        {
            if (migrationBuilder == null) throw new ArgumentNullException(nameof(migrationBuilder));
            ModelFixer.FixMigrationUp<T>(migrationBuilder, shamanOptions);
        }

        public static void FixOnModelCreating(this DbContext context, ModelBuilder modelBuilder, DbContext dbContextInstance, ShamanOptions shamanOptions = null)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));
            ModelFixer.FixOnModelCreating(modelBuilder, context.GetType(), dbContextInstance, shamanOptions);
        }

        public static bool FixOnModelCreating<T>(this ModelBuilder modelBuilder, T dbContextInstance, ShamanOptions shamanOptions = null) where T : DbContext
        {
            if (modelBuilder == null) throw new ArgumentNullException(nameof(modelBuilder));
            return ModelFixer.FixOnModelCreating(modelBuilder, typeof(T), dbContextInstance, shamanOptions);
        }

        #endregion
    }
}