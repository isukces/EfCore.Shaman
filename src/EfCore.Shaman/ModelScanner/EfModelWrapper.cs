#region using

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

#endregion

namespace EfCore.Shaman.ModelScanner
{
    public class EfModelWrapper
    {
        #region Static Methods

        public static EfModelWrapper FromModel(IModel model)
        {
            var rel = model?.Relational();
            return new EfModelWrapper
            {
                DefaultSchema = rel?.DefaultSchema?.Trim(),
                EntityTypes =
                    model?.GetEntityTypes()
                        .Select(EfModelEntityWrapper.FromIEntityType).ToList() ??
                    new List<EfModelEntityWrapper>()
            };
        }

        #endregion

        #region Properties

        public IReadOnlyList<EfModelEntityWrapper> EntityTypes { get; private set; }

        public string DefaultSchema { get; private set; }

        #endregion

        #region Nested

        public class EfModelEntityWrapper
        {
            #region Static Methods

            public static EfModelEntityWrapper FromIEntityType(IEntityType entityType)
            {
                return new EfModelEntityWrapper
                {
                    ClrType = entityType.ClrType,
                    TableName = entityType.Relational().TableName
                };
            }

            #endregion

            #region Instance Methods

            public override string ToString() => $"{ClrType?.Name} => {TableName}";

            #endregion

            #region Properties

            public Type ClrType { get; private set; }
            public string TableName { get; private set; }

            #endregion
        }

        #endregion
    }
}