#region using

using System;
using EfCore.Shaman.Services;

#endregion

namespace EfCore.Shaman
{
    public static class FixerOptionsExtension
    {
        #region Static Methods

        public static ShamanOptions With<T>(this ShamanOptions options) where T : IShamanService, new()
        {
            if (options == null) throw new ArgumentNullException(nameof(options));
            options.Services.Add(new T());
            return options;
        }


        /// <summary>
        ///     Inlcude support for WithColumnAttribute
        /// </summary>
        /// <param name="options"></param>
        /// <returns></returns>
        public static ShamanOptions WithColumnAttribute(this ShamanOptions options)
        {
            return options.With<ColumnAttributeUpdater>();
        }


        /// <summary>
        ///     Include support for TableAttribute, ColumnAttribute, NotMappedAttribute,
        ///     IndexAttribute and UniqueIndexAttribute
        /// </summary>
        /// <param name="options"></param>
        /// <returns></returns>
        public static ShamanOptions WithDefaultServices(this ShamanOptions options)
        {
            return options
                .WithColumnAttribute()
                .WithNotMappedAttribute()
                .WithIndexAttribute()
                .WithTableAttribute();
        }


        /// <summary>
        ///     Include support for IndexAttribute and UniqueIndexAttribute
        /// </summary>
        /// <param name="options"></param>
        /// <returns></returns>
        public static ShamanOptions WithIndexAttribute(this ShamanOptions options)
        {
            return options.With<IndexAttributeUpdater>();
        }


        /// <summary>
        ///     Include support for NotMappedAttribute
        /// </summary>
        /// <param name="options"></param>
        /// <returns></returns>
        public static ShamanOptions WithNotMappedAttribute(this ShamanOptions options)
        {
            return options.With<NotMappedAttributeUpdater>();
        }

        /// <summary>
        ///     Include support for TableAttribute
        /// </summary>
        /// <param name="options"></param>
        /// <returns></returns>
        public static ShamanOptions WithTableAttribute(this ShamanOptions options)
        {
            return options.With<TableAttributeUpdater>();
        }

        #endregion
    }
}