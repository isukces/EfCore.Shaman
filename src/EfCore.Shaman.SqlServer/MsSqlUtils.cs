#region using

using System;
using System.Globalization;

#endregion

namespace EfCore.Shaman.SqlServer
{
    internal class MsSqlUtils
    {
        #region Static Methods

        public static string Escape(string identifier)
        {
            return "[" + identifier + "]";
        }

        public static string Escape(string schema, string tableName)
        {
            if (String.IsNullOrEmpty(schema))
                schema = DefaultSchema;
            return Escape(schema) + "." + Escape(tableName);
        }

        public static string GetStringLength(int? maxLength)
        {
            return maxLength?.ToString(CultureInfo.InvariantCulture) ?? "max";
        }

        #endregion

        #region Other

        public const string DefaultSchema = "dbo";

        #endregion

        public static bool IsSupportedProvider(string provider)
        {
            return String.Equals(provider, "Microsoft.EntityFrameworkCore.SqlServer", StringComparison.OrdinalIgnoreCase);
        }
    }
}