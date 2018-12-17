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
                .GetCustomAttribute<SqlServerCollationAttribute>()?.Collation?.Trim();
        }

        private static string GetCollation(Type type)
        {
            return type
                .GetTypeInfo()
                .GetCustomAttribute<SqlServerCollationAttribute>()?.Collation?.Trim();
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
        public void UpdateColumnInfoForMigrationFixer(ISimpleModelInfo modelInfo, IDbSetInfo dbSetInfo,
            ColumnInfo columnInfo,
            EntityTypeBuilder entityBuilder,
            IShamanLogger logger)
        {
            const string source = nameof(SqlServerReflectionService) + "." + nameof(UpdateColumnInfoForMigrationFixer);
            if (!UseDataType) return;
            if (columnInfo.ClrProperty != typeof(string))
                return;
            var collation = GetCollation(columnInfo, dbSetInfo, modelInfo);
                        
            if (string.IsNullOrEmpty(collation))
                return;
            var isUnicode   = columnInfo.IsUnicode ?? modelInfo.DefaultIsUnicodeText;
            var sqlDataType = SqlServerFixerService.MkStringType(isUnicode, columnInfo.MaxLength, collation);
            var action      = $"{columnInfo.PropertyName}.HasColumnType(\"{sqlDataType}\")";
            logger.LogFix(source, dbSetInfo.EntityType, action);
            entityBuilder.Property(columnInfo.PropertyName).HasColumnType(sqlDataType);
        }

        public void UpdateColumnInfoInModelInfo(ColumnInfo columnInfo,
            IDbSetInfo dbSetInfo, IShamanLogger logger)
        {
            var propertyInfo = columnInfo.ClrProperty;            
            if (propertyInfo?.PropertyType != typeof(string))
                return;
            var          collation = GetCollation(propertyInfo);
            var target = $"column {dbSetInfo.GetSqlTableName()}.{columnInfo.ColumnName}";
            UpdateAnnotation(columnInfo, collation, logger, target);
        }

        public void UpdateDbSetInfo(DbSetInfo dbSetInfo, Type entityType, Type contextType, IShamanLogger logger)
        {
            if (string.IsNullOrEmpty(dbSetInfo.Schema))
                dbSetInfo.Schema = "dbo";

            var collation = GetCollation(entityType);
            var target = $"table {dbSetInfo.GetSqlTableName()}";
            UpdateAnnotation(dbSetInfo, collation, logger, target);
        }

        public void UpdateModel(IUpdatableSimpleModelInfo simpleModelInfo, Type dbContextType, IShamanLogger logger)
        {
            var collation = GetCollation(dbContextType);
            var target    = $"DbContext '{dbContextType.Name}'";
            UpdateAnnotation(simpleModelInfo, collation, logger, target);
        }

        /// <summary>
        ///     If true then
        /// </summary>
        public bool UseDataType { get; set; }
        public const string Prefix = "SqlServer.";
        public const string Ck = Prefix + "Collation";
    }
}