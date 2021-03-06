﻿#region using

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;

#endregion

namespace EfCore.Shaman
{
    public static class ShamanExtension
    {
        public static void FixMigrationUp<T>(this MigrationBuilder migrationBuilder, ShamanOptions shamanOptions = null)
            where T : DbContext
        {
            if (migrationBuilder == null) throw new ArgumentNullException(nameof(migrationBuilder));
            MigrationFixer.FixMigrationUp<T>(migrationBuilder, shamanOptions);
        }

        /// <summary>
        ///     Call this method at the end of <see cref="DbContext.OnModelCreating" />
        /// </summary>
        /// <param name="context"></param>
        /// <param name="modelBuilder"></param>
        /// <param name="shamanOptions"></param>
        public static void FixOnModelCreating(this DbContext context, ModelBuilder modelBuilder,
            ShamanOptions shamanOptions = null)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));
            ModelCreatingFixer.FixOnModelCreating(modelBuilder, context.GetType(), shamanOptions);
        }

        /// <summary>
        ///     Call this method at the end of <see cref="DbContext.OnModelCreating" />
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="modelBuilder"></param>
        /// <param name="shamanOptions"></param>
        public static void FixOnModelCreating<T>(this ModelBuilder modelBuilder, ShamanOptions shamanOptions = null)
            where T : DbContext
        {
            if (modelBuilder == null) throw new ArgumentNullException(nameof(modelBuilder));
            ModelCreatingFixer.FixOnModelCreating(modelBuilder, typeof(T), shamanOptions);
        }
       
    }
}