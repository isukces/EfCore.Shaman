using System;
using System.Collections.Generic;

namespace EfCore.Shaman.ModelScanner
{
    public struct FullTableName : IFullTableName, IEquatable<FullTableName>
    {
        public override string ToString()
        {
            if (IsEmpty) return string.Empty;
            if (string.IsNullOrEmpty(Schema)) return TableName;
            return Schema + "." + TableName;
        }
        
        public FullTableName(string tableName, string schema = null)
        {
            TableName = tableName;
            Schema    = schema;
        }

        public static bool operator ==(FullTableName left, FullTableName right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(FullTableName left, FullTableName right)
        {
            return !left.Equals(right);
        }

        public bool Equals(FullTableName other)
        {
            if (IsEmpty) return other.IsEmpty;
            return Comparer.Equals(TableName ?? "", other.TableName ?? "")
                   && Comparer.Equals(Schema ?? "", other.Schema ?? "");
        }

        public override bool Equals(object other)
        {
            return !ReferenceEquals(null, other) && other is FullTableName x && Equals(x);
        }

        public override int GetHashCode()
        {
            if (IsEmpty)
                return 0;
            unchecked
            {
                return (Comparer.GetHashCode(TableName ?? "") * 397) ^ Comparer.GetHashCode(Schema ?? "");
            }
        }
        
        public FullTableName WithDefaultSchema(string defaultSchema)
        {
            if (IsEmpty || !string.IsNullOrEmpty(Schema))
                return this;
            return new FullTableName(TableName, defaultSchema);
        }


        public string TableName { get; }
        public string Schema    { get; }

        public static FullTableName Empty
        {
            get { return new FullTableName(); }
        }

        public bool IsEmpty
        {
            get { return string.IsNullOrEmpty(TableName); }
        }
#if FXCORE
        public static IEqualityComparer<string> Comparer = StringComparer.OrdinalIgnoreCase;
#else
        public static IEqualityComparer<string> Comparer = StringComparer.InvariantCultureIgnoreCase;
#endif
    }
}