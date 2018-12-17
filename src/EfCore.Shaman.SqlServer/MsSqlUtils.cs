using System;
using System.Globalization;

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

        public static string GetStringLength(int? maxLength)
            => maxLength?.ToString(CultureInfo.InvariantCulture) ?? "max";

        public static string GetSqlTableName(this IDbSetInfo tn)
        {
            return Escape(tn.Schema, tn.TableName);
        }

        public static bool IsSupportedProvider(string provider)
            => string.Equals(provider, "Microsoft.EntityFrameworkCore.SqlServer", StringComparison.OrdinalIgnoreCase);

        public static string QuoteText(string name)
            => name == null ? "NULL" : $"\'{name.Replace("'", "''")}\'";

        public const string DefaultSchema = "dbo";
    }
}