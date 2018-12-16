using System;
using System.Reflection;
using EfCore.Shaman.ModelScanner;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EfCore.Shaman.SqlServer
{
    class SqlServerReflectionService : IColumnInfoUpdateService, IDbSetInfoUpdateService
    {
        public void ModelInfoUpdateColumnInfo(ColumnInfo columnInfo, PropertyInfo propertyInfo, IShamanLogger logger)
        {
            var attribute = propertyInfo.GetCustomAttribute<SqlServerCollationAttribute>();
            var collation = attribute?.Collation?.Trim();
            if (string.IsNullOrEmpty(collation)) return;
            logger.Log(typeof(SqlServerReflectionService), nameof(ModelFixerUpdateColumnInfo),
                $"Set annotation['{Ck}']='{collation}' for column '{columnInfo.ColumnName}'");
            columnInfo.Annotations[Ck] = collation;
        }

        public void ModelFixerUpdateColumnInfo(ColumnInfo columnInfo, EntityTypeBuilder entityBuilder, Type entityType,
            IShamanLogger logger)
        {            
        }

        public const string Prefix = "SqlServer.";
        public const string Ck = Prefix + "Collation";

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
    }
}