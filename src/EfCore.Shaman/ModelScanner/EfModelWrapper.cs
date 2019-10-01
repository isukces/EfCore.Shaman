using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace EfCore.Shaman.ModelScanner
{
    public class EfModelWrapper
    {
        public static EfModelWrapper FromModel(IModel model)
        {
            var rel = model?.Relational();
            var de  = rel?.DefaultSchema?.Trim();
            var et = model?.GetEntityTypes() ?? new List<IEntityType>();
            return new EfModelWrapper
            {
                DefaultSchema = de,
                EntityTypes = et.Select(a => EfModelEntityWrapper.FromIEntityType(a, de)).ToList()
            };
        }

        public IReadOnlyList<EfModelEntityWrapper> EntityTypes { get; private set; }

        public string DefaultSchema { get; private set; }

        public class EfModelEntityWrapper
        {
            public static EfModelEntityWrapper FromIEntityType(IEntityType entityType, string defaultSchema)
            {
                var rel = entityType.Relational();
                var relSchema = rel.Schema;
                return new EfModelEntityWrapper
                {
                    ClrType        = entityType.ClrType,
                    ShortTableName = rel.TableName,
                    Schema         = string.IsNullOrEmpty(relSchema) ? defaultSchema : relSchema
                };
            }

            public override string ToString()
            {
                var fullTableName = GetFullTableName().ToString();
                return $"Type: {ClrType?.Name}, Table: {fullTableName}";
            }

            public Type   ClrType        { get; private set; }
            public string ShortTableName { get; private set; }
            public string Schema         { get; private set; }

            public FullTableName GetFullTableName()
            {
                return new FullTableName(ShortTableName, Schema);
            }
        }
    }
}