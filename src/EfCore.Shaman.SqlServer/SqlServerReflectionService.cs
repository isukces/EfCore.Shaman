using System;
using System.Reflection;
using EfCore.Shaman.ModelScanner;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EfCore.Shaman.SqlServer
{
    public sealed class SqlServerReflectionService : IColumnInfoUpdateService,
        IDbSetInfoUpdateService,
        IModelPrepareService
    {
        private static string GetCollation(MemberInfo memberInfo)
        {
            return memberInfo
                .GetCustomAttribute<SqlServerCollationAttribute>()?
                .Collation?.Trim();
        }

        private static void UpdateAnnotation(IShamanAnnotatable annotatable, string collation, IShamanLogger logger,
            string target)
        {
            if (string.IsNullOrEmpty(collation))
                return;
            logger.Log(nameof(SqlServerReflectionService),
                $"Set annotation['{Ck}']='{collation}' for {target}");
            annotatable.Annotations[Ck] = collation;
        }

        /// <summary>
        /// Gets collation from annotation but not checks if column is string so check this elsewhere
        /// </summary>
        /// <param name="sources"></param>
        /// <returns></returns>
        public static string GetCollation(params IShamanAnnotatable[] sources)
        {
            foreach (var annotatable in sources)
            {
                if (!annotatable.Annotations.TryGetValue(Ck, out var annotation)) continue;
                var collation = annotation as string;
                if (!string.IsNullOrEmpty(collation))
                    return collation;
            }

            return null;
        }

        public void UpdateColumnInfoOnModelCreating(IDbSetInfo dbSetInfo,
            ColumnInfo columnInfo,
            EntityTypeBuilder entityBuilder,
            IShamanLogger logger)
        {
            const string source = nameof(SqlServerReflectionService) + "." + nameof(UpdateColumnInfoOnModelCreating);
            if (!UseDataType) return;
            if (columnInfo.ClrProperty?.PropertyType != typeof(string))
                return;
            var modelInfo = dbSetInfo.Model;
            var collation = GetCollation(columnInfo, dbSetInfo, modelInfo);

            if (string.IsNullOrEmpty(collation))
            {
                logger.Log(source, $"Column {dbSetInfo.GetSqlTableName()}.{columnInfo.PropertyName} has no SQL collation info");
                return;
            }

            var isUnicode   = columnInfo.IsUnicode ?? modelInfo.DefaultIsUnicodeText;
            var sqlDataType = SqlServerFixerService.MkStringType(isUnicode, columnInfo.MaxLength, collation);
            var action      = $"{columnInfo.PropertyName}.HasColumnType(\"{sqlDataType}\")";
            logger.LogCalling(source, dbSetInfo.EntityType, action);
            entityBuilder.Property(columnInfo.PropertyName).HasColumnType(sqlDataType);
        }

        public void UpdateColumnInfoInModelInfo(ColumnInfo columnInfo,
            IDbSetInfo dbSetInfo, IShamanLogger logger)
        {
            var propertyInfo = columnInfo.ClrProperty;
            if (propertyInfo?.PropertyType != typeof(string))
                return;
            var collation = GetCollation(propertyInfo);
            if (string.IsNullOrEmpty(collation))
                return;
            var target = $"column {dbSetInfo.GetSqlTableName()}.{columnInfo.ColumnName}";
            UpdateAnnotation(columnInfo, collation, logger, target);
        }

        public void UpdateDbSetInfo(DbSetInfo dbSetInfo, Type entityType, Type contextType, IShamanLogger logger)
        {
            var target = $"table {dbSetInfo.GetSqlTableName()}";
            if (string.IsNullOrEmpty(dbSetInfo.Schema))
            {
                dbSetInfo.Schema = UseDefaultTableSchema;
                logger.Log(nameof(SqlServerReflectionService),
                    $"Set table schema '{dbSetInfo.Schema}' for {target}, entity={entityType}");
            }

            var collation = GetCollation(entityType.GetTypeInfo());
            if (string.IsNullOrEmpty(collation))
                return;
            UpdateAnnotation(dbSetInfo, collation, logger, target);
        }

        public void UpdateModel(IUpdatableSimpleModelInfo simpleModelInfo, Type dbContextType, IShamanLogger logger)
        {
            var collation = GetCollation(dbContextType.GetTypeInfo());
            var target    = $"DbContext '{dbContextType.Name}'";
            UpdateAnnotation(simpleModelInfo, collation, logger, target);
        }

        /// <summary>
        ///     If true then 
        /// </summary>
        public bool UseDataType { get; set; }

        public const string Prefix = "SqlServer.";
        public const string Ck = Prefix + "Collation";

        public string UseDefaultTableSchema { get; set; } = MsSqlUtils.DefaultSchema;
    }
}