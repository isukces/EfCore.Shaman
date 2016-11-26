#region using

using System;
using System.Linq;
using EfCore.Shaman.ModelScanner;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Migrations.Operations;

#endregion

namespace EfCore.Shaman
{
    public class ModelFixer
    {
        #region Constructors

        private ModelFixer(Type type, ShamanOptions shamanOptions)
        {
            _type = type;
            _shamanOptions = shamanOptions ?? new ShamanOptions();
            _info = new ModelInfo(type, _shamanOptions.Services);
        }

        #endregion

        #region Static Methods

        public static void FixMigrationUp<T>(MigrationBuilder migrationBuilder, ShamanOptions shamanOptions = null) where T : DbContext
        {
            var tmp = new ModelFixer(typeof(T), shamanOptions);
            tmp.FixOnModelCreating(migrationBuilder);
        }

        public static void FixOnModelCreating(ModelBuilder modelBuilder, Type contextType, ShamanOptions shamanOptions = null)
        {
            var modelFixer = new ModelFixer(contextType, shamanOptions);
            modelFixer.FixOnModelCreatingInternal(modelBuilder);
        }


        private static int ComparePropertyWrapper(ColumnInfo a, ColumnInfo b)
        {
            if ((a == null) && (b == null)) return 0;
            if (a == null) return -1;
            if (b == null) return 1;

            var c = a.ForceFieldOrder.CompareTo(b.ForceFieldOrder);
            if (c != 0) return c;
            c = a.ReflectionIndex.CompareTo(b.ReflectionIndex);
            if (c != 0) return c;
            return 0; // should never occur
        }

        private static void TrySetIndexName(IndexBuilder indexBuilder, string indexName)
        {
            indexName = indexName?.Trim();
            if (string.IsNullOrEmpty(indexName) || indexName.StartsWith("@")) return;
            indexBuilder.HasName(indexName);
        }

        #endregion

        #region Instance Methods

        private void FixOnModelCreating(MigrationBuilder migrationBuilder)
        {
            foreach (var table in migrationBuilder.Operations.OfType<CreateTableOperation>())
                FixOnModelCreatingForTable(table);
        }

        private void FixOnModelCreatingForTable(CreateTableOperation table)
        {
            var entity = _info.GetByTableName(table.Name);
            var colsDic = entity
                .Properites
                .Where(a => !a.IsNotMapped)
                .ToDictionary(a => a.ColumnName, StringComparer.OrdinalIgnoreCase);
            var natural = Enumerable.Range(0, table.Columns.Count).ToDictionary(a => table.Columns[a].Name, a => a);
            table.Columns.Sort((a, b) =>
            {
                ColumnInfo aWrapper, bWrapper;
                colsDic.TryGetValue(a.Name, out aWrapper);
                colsDic.TryGetValue(b.Name, out bWrapper);
                var compareResult = ComparePropertyWrapper(aWrapper, bWrapper);
                if (compareResult != 0)
                    return compareResult;
                return natural[a.Name].CompareTo(natural[b.Name]); // bez zmian
            });
        }

        private void FixOnModelCreatingInternal(ModelBuilder modelBuilder)
        {
            foreach (var dbSet in _info.DbSets)
            {
                var allIdx = dbSet.Properites
                    .SelectMany(q => q.ColumnIndexes.Select(w => Tuple.Create(w, q.ColumnName)))
                    .GroupBy(a => a.Item1.IndexName, StringComparer.OrdinalIgnoreCase);
                foreach (var idx in allIdx)
                {
                    var fields = idx.OrderBy(q => q.Item1.Order).Select(q => q.Item2).ToArray();
                    var entity = modelBuilder.Entity(dbSet.EntityType);
                    var indexBuilder = entity.HasIndex(fields);
                    TrySetIndexName(indexBuilder, idx.Key);
                    indexBuilder.IsUnique(idx.First().Item1.IsUnique);

                }
            }
        }

        #endregion

        #region Fields

        private readonly Type _type;
        private ModelInfo _info;
        private ShamanOptions _shamanOptions;

        #endregion
    }
}