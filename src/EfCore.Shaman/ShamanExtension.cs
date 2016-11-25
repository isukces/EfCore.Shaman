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

        public static void FixMigrationUp<T>(this MigrationBuilder migrationBuilder) where T : DbContext
        {
            if (migrationBuilder == null) throw new ArgumentNullException(nameof(migrationBuilder));
            ModelFixer.FixMigrationUp<T>(migrationBuilder);
        }

        public static void FixOnModelCreating(this DbContext context, ModelBuilder modelBuilder)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));
            ModelFixer.FixOnModelCreating(modelBuilder, context.GetType());
        }

        public static void FixOnModelCreating<T>(this ModelBuilder modelBuilder) where T : DbContext
        {
            if (modelBuilder == null) throw new ArgumentNullException(nameof(modelBuilder));
            ModelFixer.FixOnModelCreating(modelBuilder, typeof(T));
        }

        #endregion
    }
}