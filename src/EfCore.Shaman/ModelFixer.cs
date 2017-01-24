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

        public ModelFixer(Type dbContextType, ShamanOptions shamanOptions, ModelInfo modelInfo = null)
        {
            _shamanOptions = shamanOptions ?? ShamanOptions.CreateShamanOptions(dbContextType);
            _info = modelInfo ?? new ModelInfo(dbContextType, _shamanOptions.Services);
        }

        #endregion

        #region Static Methods

        public static void FixMigrationUp<T>(MigrationBuilder migrationBuilder, ShamanOptions shamanOptions = null)
            where T : DbContext
        {
            var tmp = new ModelFixer(typeof(T), shamanOptions);
            tmp.FixMigrationUp(migrationBuilder);
        }

        public static bool FixOnModelCreating(ModelBuilder modelBuilder, Type contextType, DbContext dbContextInstance,
            ShamanOptions shamanOptions = null)
        {
            var x = dbContextInstance as IShamanFriendlyDbContext;
            if (x != null && x.CreationMode == DbContextCreationMode.WithoutModelFixing)
                return false;
            var modelFixer = new ModelFixer(contextType, shamanOptions);
            modelFixer.FixOnModelCreating(modelBuilder);
            return true;
        }


        private static int ComparePropertyWrapper(ColumnInfo a, ColumnInfo b)
        {
            if (a == null && b == null) return 0;
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

        public void FixMigrationUp(MigrationBuilder migrationBuilder)
        {
            var services = _shamanOptions?.Services.OfType<IFixMigrationUpService>().ToArray();
            if (services != null)
                foreach (var i in services)
                    i.FixMigrationUp(migrationBuilder, _info);

            foreach (var table in migrationBuilder.Operations.OfType<CreateTableOperation>())
                FixOnModelCreatingForTable(table);
        }

        public void FixOnModelCreating(ModelBuilder modelBuilder)
        {
            if (!string.IsNullOrEmpty(_info.DefaultSchema))
                modelBuilder = modelBuilder.HasDefaultSchema(_info.DefaultSchema);

            var dict = modelBuilder.Model.GetEntityTypes().ToDictionary(a => a.ClrType, a => a.Relational());

            foreach (var dbSet in _info.DbSets)
            {
                var entity = modelBuilder.Entity(dbSet.EntityType);
                foreach (var idx in dbSet.Indexes)
                {
                    var fields = idx.Fields.Select(a => a.FieldName).ToArray();
                    var indexBuilder = entity.HasIndex(fields);
                    TrySetIndexName(indexBuilder, idx.IndexName);
                    indexBuilder.IsUnique(idx.IsUnique);
                }
                foreach (var i in dbSet.Properites)
                {
                    if (i.MaxLength == null || i.DecimalPlaces == null)
                        continue;
                    var type = $"decimal({i.MaxLength},{i.DecimalPlaces})";
                    entity.Property(i.PropertyName).HasColumnType(type);
                }
            }
        }

        private void FixOnModelCreatingForTable(CreateTableOperation table)
        {
            if (table == null)
                throw new ArgumentNullException(nameof(table));
            if (_info == null)
                throw new NullReferenceException(nameof(_info));
            var entity = _info.GetByTableName(table.Name);
            if (entity == null)
                throw new Exception($"Unable to find table {table.Name}.");
            var colsDic = entity
                .Properites
                .Where(info => !info.IsNotMapped && !info.IsNavigationProperty)
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

        #endregion

        #region Fields

        private readonly ModelInfo _info;
        private readonly ShamanOptions _shamanOptions;

        #endregion
    }
}