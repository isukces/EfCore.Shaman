using System;
using System.Linq;
using EfCore.Shaman.ModelScanner;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Migrations.Operations;

namespace EfCore.Shaman
{
    public class MigrationFixer
    {
        public MigrationFixer(Type dbContextType, ShamanOptions shamanOptions, ModelInfo modelInfo = null)
        {
            _shamanOptions = shamanOptions ?? ShamanOptions.CreateShamanOptions(dbContextType);
            _info          = modelInfo ?? new ModelInfo(dbContextType, _shamanOptions);
        }

        public static void FixMigrationUp<T>(MigrationBuilder migrationBuilder, ShamanOptions shamanOptions = null, [CanBeNull]GetModelTableNameDelegate getModelTableName=null)
            where T : DbContext
        {
            var tmp = new MigrationFixer(typeof(T), shamanOptions);
            tmp.FixMigrationUp(migrationBuilder, getModelTableName);
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

        public void FixMigrationUp(MigrationBuilder migrationBuilder,  [CanBeNull]GetModelTableNameDelegate getModelTableName)
        {
            var services = _shamanOptions?.Services.OfType<IFixMigrationUpService>().ToArray();
            if (services != null)
                foreach (var service in services)
                {
                    Log(nameof(FixMigrationUp), $"calling service {service}");
                    service.FixMigrationUp(migrationBuilder, _info);
                }

            foreach (var table in migrationBuilder.Operations.OfType<CreateTableOperation>())
                FixOnModelCreatingForTable(table, getModelTableName);
        }


        private void FixOnModelCreatingForTable(CreateTableOperation table, [CanBeNull]GetModelTableNameDelegate getModelTableName)
        {
            if (table == null)
                throw new ArgumentNullException(nameof(table));
            if (_info == null)
                throw new NullReferenceException(nameof(_info));
            var tn = new FullTableName(table.Name, table.Schema).WithDefaultSchema(_info.DefaultSchema);
            if (getModelTableName != null)
                tn = getModelTableName(tn);
            var entity = _info.GetByTableName(tn);
            if (entity == null)
            {
                _shamanOptions.Logger.Log(typeof(MigrationFixer), nameof(FixOnModelCreatingForTable),
                    $"Table {table.Name} not found. Skipping.");
                return;
            }

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

        private void Log(string methodName, string message)
        {
            _shamanOptions.Logger.Log(typeof(MigrationFixer), methodName, message);
        }

        private readonly ModelInfo _info;
        private readonly ShamanOptions _shamanOptions;
    }

    public delegate FullTableName GetModelTableNameDelegate(FullTableName createTableName);
}