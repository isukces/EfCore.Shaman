#region using

using System;
using EfCore.Shaman.Services;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.EntityFrameworkCore.Storage;

#endregion

namespace EfCore.Shaman
{
    public static class ShamanOptionsExtensions
    {
        #region Static Methods

        /// <summary>
        ///     Inspired by source code of static method
        ///     Microsoft.EntityFrameworkCore.RelationalDatabaseFacadeExtensions.ExecuteSqlCommand
        /// </summary>
        /// <param name="databaseFacade"></param>
        /// <param name="sql"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public static RelationalDataReader ExecuteReader(
            this DatabaseFacade databaseFacade,
            string sql,
            params object[] parameters)
        {
            //  Check.NotNull(databaseFacade, nameof(databaseFacade));

            var concurrencyDetector = databaseFacade.GetService<IConcurrencyDetector>();

            using (concurrencyDetector.EnterCriticalSection())
            {
                var rawSqlCommand = databaseFacade
                    .GetService<IRawSqlCommandBuilder>()
                    .Build(sql, parameters);

                return rawSqlCommand
                    .RelationalCommand
                    .ExecuteReader(GetRelationalConnection(databaseFacade),
                        parameterValues: rawSqlCommand.ParameterValues);
            }
        }

        public static ShamanOptions With<T>(this ShamanOptions options) where T : IShamanService, new()
        {
            if (options == null) throw new ArgumentNullException(nameof(options));
            options.Services.Add(new T());
            return options;
        }

        public static ShamanOptions With(this ShamanOptions options, IShamanService service)
        {
            if (options == null) throw new ArgumentNullException(nameof(options));
            if (service == null) throw new ArgumentNullException(nameof(service));
            options.Services.Add(service);
            return options;
        }


        /// <summary>
        ///     Include support for TableAttribute, ColumnAttribute, NotMappedAttribute,
        ///     RequiredAttribute, MaxLengthAttribute, IndexAttribute, UniqueIndexAttribute
        ///     and other
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
                .With<DefaultValueAttributeUpdater>()
                .With<DefaultValueSqlAttributeUpdater>()
                .With<TableAttributeUpdater>();
        }


        /// <summary>
        ///     Copy of private static method
        ///     Microsoft.EntityFrameworkCore.RelationalDatabaseFacadeExtensions.GetRelationalConnection
        /// </summary>
        /// <param name="databaseFacade"></param>
        /// <returns></returns>
        private static IRelationalConnection GetRelationalConnection(this DatabaseFacade databaseFacade)
            => databaseFacade.GetService<IRelationalConnection>();

        #endregion
    }
}