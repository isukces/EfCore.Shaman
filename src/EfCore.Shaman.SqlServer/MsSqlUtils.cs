using System;
using System.Globalization;
using JetBrains.Annotations;

namespace EfCore.Shaman.SqlServer
{
    internal static class MsSqlUtils
    {
        public static string Escape(string identifier)
        {
            return "[" + identifier + "]";
        }

        public static string Escape(string schema, string tableName)
        {
            if (string.IsNullOrEmpty(schema))
                schema = DefaultSchema;
            return Escape(schema) + "." + Escape(tableName);
        }

        public static string GetSqlTableName(this IDbSetInfo tn)
        {
            return Escape(tn.Schema, tn.TableName);
        }

        public static string GetStringLength(int? maxLength)
        {
            return maxLength?.ToString(CultureInfo.InvariantCulture) ?? "max";
        }

        public static bool IsSupportedProvider(string provider)
        {
            return string.Equals(provider, "Microsoft.EntityFrameworkCore.SqlServer",
                StringComparison.OrdinalIgnoreCase);
        }

        public static bool IsTextField([CanBeNull] string msSqlColType)
        {
            if (string.IsNullOrEmpty(msSqlColType))
                return false;
            return IsUnicodeTextField(msSqlColType)
                   || msSqlColType.StartsWith("varchar", StringComparison.OrdinalIgnoreCase)
                   || msSqlColType.StartsWith("char", StringComparison.OrdinalIgnoreCase);
        }

        public static bool IsUnicodeTextField([CanBeNull] string msSqlColType)
        {
            if (string.IsNullOrEmpty(msSqlColType))
                return false;
            return msSqlColType.StartsWith("nvarchar", StringComparison.OrdinalIgnoreCase)
                   || msSqlColType.StartsWith("nchar", StringComparison.OrdinalIgnoreCase);
        }

        public static string QuoteText(string name)
        {
            return name == null ? "NULL" : $"\'{name.Replace("'", "''")}\'";
        }

        public const string DefaultSchema = "dbo";
    }
}