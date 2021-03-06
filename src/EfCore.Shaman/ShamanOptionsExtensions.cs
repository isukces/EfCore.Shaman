﻿#region using

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EfCore.Shaman.Services;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
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
        public static Task<RelationalDataReader> ExecuteReaderAsync(
            this DatabaseFacade databaseFacade,
            string sql,
            params object[] parameters)
        {
            //  Check.NotNull(databaseFacade, nameof(databaseFacade));

            var concurrencyDetector = databaseFacade.GetService<IConcurrencyDetector>();

            using(concurrencyDetector.EnterCriticalSection())
            {
                var rawSqlCommand = databaseFacade
                    .GetService<IRawSqlCommandBuilder>()
                    .Build(sql, parameters);

              
                //    [NotNull] 
                var connection = GetRelationalConnection(databaseFacade);
                //    [CanBeNull] 
                Dictionary<string, object> parameterValues = null;
                /*if (parameters.Any())
                {
                    parameterValues = new Dictionary<string, object>();
                    foreach(var i in parameters)
                        
                }*/
                
                //    [CanBeNull] 
                IReadOnlyList<ReaderColumn > readerColumns = null;
                //    [CanBeNull] 
                DbContext context = null;
                //    [CanBeNull] 
                IDiagnosticsLogger<DbLoggerCategory.Database.Command> logger = null;
                    
                    
                    
                var parameterObject =new RelationalCommandParameterObject(
                    connection,
                    parameterValues,
                    readerColumns,
                    context,
                    logger
                    
                    );
                //RelationalCommandParameterObject eee = GetRelationalConnection(databaseFacade);
                return rawSqlCommand.RelationalCommand.ExecuteReaderAsync(parameterObject);
            }
        }
        /*
         *
         *  public RelationalCommandParameterObject(
        [NotNull] IRelationalConnection connection,
        [CanBeNull] IReadOnlyDictionary<string, object> parameterValues,
        [CanBeNull] IReadOnlyList<ReaderColumn> readerColumns,
        [CanBeNull] DbContext context,
        [CanBeNull] IDiagnosticsLogger<DbLoggerCategory.Database.Command> logger)
        {
            Check.NotNull(connection, nameof(connection));

            Connection = connection;
            ParameterValues = parameterValues;
            ReaderColumns = readerColumns;
            Context = context;
            Logger = logger;
        }
         * 
         */

        public static ShamanOptions With<T>(this ShamanOptions options) where T : IShamanService, new()
        {
            if (options == null) throw new ArgumentNullException(nameof(options));
            return options.With(new T());
        }

        public static ShamanOptions With(this ShamanOptions options, IShamanService service)
        {
            if (options == null) throw new ArgumentNullException(nameof(options));
            if (service == null) throw new ArgumentNullException(nameof(service));
            options.Services.Add(service);
            var modificationService = service as IShamanOptionModificationService;
            modificationService?.ModifyShamanOptions(options);
            return options;
        }

        public static ShamanOptions WithLogger(this ShamanOptions options, IShamanLogger logger)
        {
            if (options == null) throw new ArgumentNullException(nameof(options));
            options.Logger = logger ?? EmptyShamanLogger.Instance;
            return options;
        }


        /// <summary>
        ///     Include support for TableAttribute, ColumnAttribute, NotMappedAttribute,
        ///     RequiredAttribute, MaxLengthAttribute, IndexAttribute, UniqueIndexAttribute
        ///     and other
        /// </summary>
        /// <param name="options"></param>
        /// <returns></returns>
        public static ShamanOptions WithDefaultServices(this ShamanOptions options, Type dbContextType)
        {
            return options
                .With<ColumnInfoColumnAttributeUpdater>()
                .With<NotMappedAttributeUpdater>()
                .With<NavigationPropertyAttributeUpdater>()
                .With(new NavigationPropertyByPropertyTypeUpdater(dbContextType))
                .With<TimestampAttributeUpdater>()
                .With<DatabaseGeneratedAttributeUpdater>()
                .With<KeyAttributeUpdater>()
                .With<IndexAttributeUpdater>()
                .With<RequiredAttributeUpdater>()
                .With<MaxLengthAttributeUpdater>()
                .With<DecimalTypeAttributeUpdater>()
                .With<UnicodeTextAttributeUpdater>()
                .With<TableAttributeUpdater>()
                .With<DefaultValueAttributeUpdater>()
                .With<DefaultValueSqlAttributeUpdater>()
                .With<DecimalPlacesColumnInfoUpdateService>()
                .With<DefaultValueColumnInfoUpdateService>()
                .With<UnicodeColumnInfoUpdateService>();
        }

        public static ShamanOptions ConfigureService<T>([NotNull] this ShamanOptions options, Action<T> configureAction)
            where T : IShamanService
        {
            if (options == null) throw new ArgumentNullException(nameof(options));
            foreach (var service in options.Services.OfType<T>())
            {
                configureAction(service);
            }

            return options;
        }

        public static ShamanOptions Without<T>([NotNull] this ShamanOptions options)
            where T : IShamanService
        {
            if (options == null) throw new ArgumentNullException(nameof(options));
            var list = options.Services;
            for (var index = list.Count - 1; index >= 0; index--)
                if (list[index].GetType() == typeof(T))
                    list.RemoveAt(index);
            return options;
        }

        /// <summary>
        ///     Copy of private static method
        ///     Microsoft.EntityFrameworkCore.RelationalDatabaseFacadeExtensions.GetRelationalConnection
        /// </summary>
        /// <param name="databaseFacade"></param>
        /// <returns></returns>
        private static IRelationalConnection GetRelationalConnection(this DatabaseFacade databaseFacade)
        {
            return databaseFacade.GetService<IRelationalConnection>();
        }

        #endregion
    }
}