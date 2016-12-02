#region using

using System;
using EfCore.Shaman.Services;

#endregion

namespace EfCore.Shaman
{
    public static class ShamanOptionsExtensions
    {
        #region Static Methods

        public static ShamanOptions With<T>(this ShamanOptions options) where T : IShamanService, new()
        {
            if (options == null) throw new ArgumentNullException(nameof(options));
            options.Services.Add(new T());
            return options;
        }


        /// <summary>
        /// Include support for TableAttribute, ColumnAttribute, NotMappedAttribute,
        /// RequiredAttribute, MaxLengthAttribute, IndexAttribute, UniqueIndexAttribute
        /// and other
        /// </summary>
        /// <param name="options"></param>
        /// <returns></returns>
        public static ShamanOptions WithDefaultServices(this ShamanOptions options)
        {
            return options
                .With<ColumnAttributeUpdater>()
                .With<NotMappedAttributeUpdater>()
                .With<ForeignKeyAttributeUpdater>()
                .With<InversePropertyAttributeUpdater>()
                .With<DatabaseGeneratedAttributeUpdater>()
                .With<KeyAttributeUpdater>()
                .With<IndexAttributeUpdater>()
                .With<RequiredAttributeUpdater>()
                .With<MaxLengthAttributeUpdater>()
                .With<DecimalTypeAttributeUpdater>()
                .With<TableAttributeUpdater>();
        }

        #endregion
    }
}