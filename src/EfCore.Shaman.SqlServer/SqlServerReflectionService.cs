using System;
using System.Reflection;
using EfCore.Shaman.ModelScanner;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EfCore.Shaman.SqlServer
{
    internal class SqlServerReflectionService : IColumnInfoUpdateService, IDbSetInfoUpdateService
    {
        public void UpdateColumnInfoForMigrationFixer(ISimpleModelInfo modelInfo, IDbSetInfo dbSetInfo, ColumnInfo columnInfo,
            EntityTypeBuilder entityBuilder,
            IShamanLogger logger)
        {
            const string source = nameof(SqlServerReflectionService) + "." + nameof(UpdateColumnInfoForMigrationFixer);
            if (!UseDataType) return;
            string collation = null;
            if (columnInfo.Annotations.TryGetValue(Ck, out var annotation))
                collation = annotation as string;
            if (string.IsNullOrEmpty(collation))
                return;
            var isUnicode   = columnInfo.IsUnicode ?? modelInfo.DefaultIsUnicodeText;
            var sqlDataType = SqlServerFixerService.MkStringType(isUnicode, columnInfo.MaxLength, collation);
            var action      = $"HasColumnType(\"{sqlDataType}\")";
            logger.LogFix(source, dbSetInfo.EntityType, action);
            entityBuilder.Property(columnInfo.PropertyName).HasColumnType(sqlDataType);
        }

        public void UpdateColumnInfoInModelInfo(ColumnInfo columnInfo, PropertyInfo propertyInfo, IShamanLogger logger)
        {
            const string source    = nameof(SqlServerReflectionService) + "." + nameof(UpdateColumnInfoInModelInfo);
            var          attribute = propertyInfo.GetCustomAttribute<SqlServerCollationAttribute>();
            var          collation = attribute?.Collation?.Trim();
            if (string.IsNullOrEmpty(collation)) return;
            logger.Log(source, $"Set annotation['{Ck}']='{collation}' for column '{columnInfo.ColumnName}'");
            columnInfo.Annotations[Ck] = collation;
        }

        public void UpdateDbSetInfo(DbSetInfo dbSetInfo, Type entityType, Type contextType, IShamanLogger logger)
        {
            if (string.IsNullOrEmpty(dbSetInfo.Schema))
                dbSetInfo.Schema = "dbo";
            var attribute = entityType.GetTypeInfo().GetCustomAttribute<SqlServerCollationAttribute>();
            var collation = attribute?.Collation?.Trim();
            if (string.IsNullOrEmpty(collation)) return;
            logger.Log(typeof(SqlServerReflectionService), nameof(UpdateDbSetInfo),
                $"Set annotation['{Ck}']='{collation}' for table '{dbSetInfo.TableName}'");
            dbSetInfo.Annotations[Ck] = collation;
        }

        /// <summary>
        ///     If true then
        /// </summary>
        public bool UseDataType { get; set; }

        public const string Prefix = "SqlServer.";
        public const string Ck = Prefix + "Collation";
    }
}