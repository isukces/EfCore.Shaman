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
        ///     Inlcude support for WithColumnAttribute
        /// </summary>
        /// <param name="options"></param>
        /// <returns></returns>
        public static ShamanOptions WithColumnAttribute(this ShamanOptions options)
        {
            return options.With<ColumnAttributeUpdater>();
        }

        /// <summary>
        ///     Include support for TableAttribute
        /// </summary>
        /// <param name="options"></param>
        /// <returns></returns>
        public static ShamanOptions WithDecimalTypeAttribute(this ShamanOptions options)
        {
            return options.With<DecimalTypeAttributeUpdater>();
        }


        /// <summary>
        ///     Include support for TableAttribute, ColumnAttribute, NotMappedAttribute,
        ///     RequiredAttribute, MaxLengthAttribute, IndexAttribute and UniqueIndexAttribute
        /// </summary>
        /// <param name="options"></param>
        /// <returns></returns>
        public static ShamanOptions WithDefaultServices(this ShamanOptions options)
        {
            return options
                .WithColumnAttribute()
                .WithNotMappedAttribute()
                .WithKeyAttribute()
                .WithIndexAttribute()
                .WithRequiredAttribute()
                .WithMaxLengthAttribute()
                .WithDecimalTypeAttribute()
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
        public static ShamanOptions WithKeyAttribute(this ShamanOptions options)
        {
            return options.With<KeyAttributeUpdater>();
        }

        /// <summary>
        ///     Include support for TableAttribute
        /// </summary>
        /// <param name="options"></param>
        /// <returns></returns>
        public static ShamanOptions WithMaxLengthAttribute(this ShamanOptions options)
        {
            return options.With<MaxLengthAttributeUpdater>();
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
        ///     Include support for RequiredAttribute
        /// </summary>
        /// <param name="options"></param>
        /// <returns></returns>
        public static ShamanOptions WithRequiredAttribute(this ShamanOptions options)
        {
            return options.With<RequiredAttributeUpdater>();
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