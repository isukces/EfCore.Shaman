#region using

using System;
using System.Collections.Generic;
using System.Globalization;
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
            _info = modelInfo ?? new ModelInfo(dbContextType, _shamanOptions);
        }

        #endregion

        #region Static Methods

        public static void FixMigrationUp<T>(MigrationBuilder migrationBuilder, ShamanOptions shamanOptions = null)
            where T : DbContext
        {
            var tmp = new ModelFixer(typeof(T), shamanOptions);
            tmp.FixMigrationUp(migrationBuilder);
        }

        public static void FixOnModelCreating(ModelBuilder modelBuilder, Type contextType,
            ShamanOptions shamanOptions = null)
        {
            shamanOptions = shamanOptions ?? ShamanOptions.CreateShamanOptions(contextType);
            Action<string> log = message =>
                shamanOptions.Logger.Log(typeof(ModelFixer), nameof(FixOnModelCreating), message);

            log("Before SetRawModel");
            try
            {
                ModelsCachedContainer.SetRawModel(contextType, modelBuilder.Model, shamanOptions.Logger);
                log("After SetRawModel");
            }
            catch (Exception e)
            {
                shamanOptions.Logger.LogException(Guid.Parse("{8E4EA170-3B75-4491-8074-177E5DC8F671}"), e);
                log("After SetRawModel with exception " + e.Message);
                throw;
            }


            var fix = fixingHolder.TryFix(contextType, () =>
            {
                log("Fixing...");
                var modelFixer = new ModelFixer(contextType, shamanOptions);
                modelFixer.FixOnModelCreating(modelBuilder);
            });
            if (!fix)
                log("Skip");
        }

        private void ChangeTableName(EntityTypeBuilder entity, DbSetInfo dbSet)
        {
            if (string.IsNullOrEmpty(dbSet.TableName)) return;
            if (string.IsNullOrEmpty(dbSet.Schema))
            {
                LogFix(nameof(ChangeTableName), dbSet.EntityType, $"ToTable(\"{dbSet.TableName}\")");
                entity.ToTable(dbSet.TableName);
            }
            else
            {
                LogFix(nameof(ChangeTableName), dbSet.EntityType, $"ToTable(\"{dbSet.TableName}\", \"{dbSet.Schema}\")");
                entity.ToTable(dbSet.TableName, dbSet.Schema);
            }
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
                foreach (var service in services)
                {
                    Log(nameof(FixMigrationUp), $"calling service {service}");
                    service.FixMigrationUp(migrationBuilder, _info);
                }

            foreach (var table in migrationBuilder.Operations.OfType<CreateTableOperation>())
                FixOnModelCreatingForTable(table);
        }

        public void FixOnModelCreating(ModelBuilder modelBuilder)
        {
            if (!string.IsNullOrEmpty(_info.DefaultSchema))
            {
                Log(nameof(FixOnModelCreating), $"calling modelBuilder.HasDefaultSchema(\"{_info.DefaultSchema}\")");
                modelBuilder = modelBuilder.HasDefaultSchema(_info.DefaultSchema);
            }
            var plugins = _shamanOptions.Services.OfType<IColumnInfoUpdateService>().ToArray();

            foreach (var dbSet in _info.DbSets)
            {
                var entity = modelBuilder.Entity(dbSet.EntityType);
                ChangeTableName(entity, dbSet);
                foreach (ITableIndexInfo idx in dbSet.Indexes)
                {
                    if (!idx.IndexType.IsNormalIndex())
                        continue;
                    var fields = idx.Fields.Select(a => a.FieldName).ToArray();
                    var indexBuilder = entity.HasIndex(fields);
                    TrySetIndexName(indexBuilder, idx.IndexName);
                    indexBuilder.IsUnique(idx.IndexType == IndexType.UniqueIndex);
#if EF200
                    if (!string.IsNullOrEmpty(idx.Filter))
                        indexBuilder.HasFilter(idx.Filter);
#endif
                }

                foreach (var info in dbSet.Properites)
                {
                    foreach(var i in plugins)
                        i.ModelFixerUpdateColumnInfo(info, entity, dbSet.EntityType, _shamanOptions.Logger);
                }
            }
        }

       

        private void FixOnModelCreatingForTable(CreateTableOperation table)
        {
            if (table == null)
                throw new ArgumentNullException(nameof(table));
            if (_info == null)
                throw new NullReferenceException(nameof(_info));
            var entity = _info.GetByTableName(new FullTableName(table.Name, table.Schema));
            if (entity == null)
            {
                _shamanOptions.Logger.Log(typeof(ModelFixer), nameof(FixOnModelCreatingForTable),
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
            _shamanOptions.Logger.Log(typeof(ModelFixer), methodName, message);
        }

        private void LogFix(string methodName, ColumnInfo columnInfo, Type entityType, string action)
        {
            string logMessage = $"calling {action} for {entityType.Name}.{columnInfo.PropertyName}";
            Log(methodName, logMessage);
        }

        private void LogFix(string methodName, Type entityType, string action)
        {
            string logMessage = $"calling {action} for {entityType.Name}";
            Log(methodName, logMessage);
        }

        
        

        #endregion

        #region Static Fields

        private static readonly FixingHolder fixingHolder = new FixingHolder();

        #endregion

        #region Fields

        private readonly ModelInfo _info;
        private readonly ShamanOptions _shamanOptions;

        #endregion

        #region Nested

        private class FixingHolder
        {
            #region Instance Methods

            public bool TryFix(Type contextType, Action action)
            {
                lock (_typesUnderProcessing)
                {
                    if (_typesUnderProcessing.Contains(contextType))
                        return false;
                    _typesUnderProcessing.Add(contextType);
                }
                try
                {
                    action();
                    return true;
                }
                finally
                {
                    lock (_typesUnderProcessing)
                    {
                        _typesUnderProcessing.Remove(contextType);
                    }
                }
            }

            #endregion

            #region Fields

            private readonly HashSet<Type> _typesUnderProcessing = new HashSet<Type>();

            #endregion
        }

        #endregion
    }
 
}