using System;
using System.Collections.Generic;
using System.Linq;
using EfCore.Shaman.ModelScanner;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EfCore.Shaman
{
    public class ModelCreatingFixer
    {
        public ModelCreatingFixer(Type dbContextType, ShamanOptions shamanOptions, ModelInfo modelInfo = null)
        {
            _shamanOptions = shamanOptions ?? ShamanOptions.CreateShamanOptions(dbContextType);
            _info          = modelInfo ?? new ModelInfo(dbContextType, _shamanOptions);
        }

        public static void FixOnModelCreating(ModelBuilder modelBuilder, Type contextType,
            ShamanOptions shamanOptions = null)
        {
            shamanOptions = shamanOptions ?? ShamanOptions.CreateShamanOptions(contextType);
            Action<string> log = delegate(string message)
            {
                shamanOptions.Logger.Log(typeof(MigrationFixer), nameof(FixOnModelCreating), message);
            };

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
                var modelFixer = new ModelCreatingFixer(contextType, shamanOptions);
                modelFixer.FixOnModelCreating(modelBuilder);
            });
            if (!fix)
                log("Skip");
        }

        private static void TrySetIndexName(IndexBuilder indexBuilder, string indexName)
        {
            indexName = indexName?.Trim();
            if (string.IsNullOrEmpty(indexName) || indexName.StartsWith("@")) return;
            indexBuilder.HasName(indexName);
        }

        public void FixOnModelCreating(ModelBuilder modelBuilder)
        {
            if (!string.IsNullOrEmpty(_info.DefaultSchema))
            {
                Log(nameof(FixOnModelCreating), $"calling modelBuilder.HasDefaultSchema(\"{_info.DefaultSchema}\")");
                modelBuilder = modelBuilder.HasDefaultSchema(_info.DefaultSchema);
            }

            var updateServices = _shamanOptions.Services.OfType<IColumnInfoUpdateService>().ToArray();

            foreach (var dbSet in _info.DbSets)
            {
                var entity = modelBuilder.Entity(dbSet.EntityType);
                ChangeTableName(entity, dbSet);
                foreach (ITableIndexInfo idx in dbSet.Indexes)
                {
                    if (!idx.IndexType.IsNormalIndex())
                        continue;
                    var fields       = idx.Fields.Select(a => a.FieldName).ToArray();
                    var indexBuilder = entity.HasIndex(fields);
                    TrySetIndexName(indexBuilder, idx.IndexName);
                    indexBuilder.IsUnique(idx.IndexType == IndexType.UniqueIndex);
#if EF200
                    if (!string.IsNullOrEmpty(idx.Filter))
                        indexBuilder.HasFilter(idx.Filter);
#endif
                }

                foreach (var columnInfo in dbSet.Properites)
                {
                    for (var index = 0; index < updateServices.Length; index++)
                    {
                        var service = updateServices[index];
                        service.UpdateColumnInfoOnModelCreating(dbSet, columnInfo, entity, _shamanOptions.Logger);
                    }
                }
            }
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
                LogFix(nameof(ChangeTableName), dbSet.EntityType,
                    $"ToTable(\"{dbSet.TableName}\", \"{dbSet.Schema}\")");
                entity.ToTable(dbSet.TableName, dbSet.Schema);
            }
        }


        private void Log(string methodName, string message)
        {
            _shamanOptions.Logger.Log(typeof(ModelCreatingFixer), methodName, message);
        }
       
        private void LogFix(string methodName, Type entityType, string action)
        {
            string logMessage = $"calling {action} for {entityType.Name}";
            Log(methodName, logMessage);
        }

        private static readonly FixingHolder fixingHolder = new FixingHolder();

        private readonly ShamanOptions _shamanOptions;
        private readonly ModelInfo _info;

        private class FixingHolder
        {
            public bool TryFix(Type contextType, Action action)
            {
                lock(_typesUnderProcessing)
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
                    lock(_typesUnderProcessing)
                    {
                        _typesUnderProcessing.Remove(contextType);
                    }
                }
            }

            private readonly HashSet<Type> _typesUnderProcessing = new HashSet<Type>();
        }
    }
}