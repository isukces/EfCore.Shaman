using System;
using System.Collections.Generic;

namespace EfCore.Shaman.ModelScanner
{
    public struct FullTableName : IFullTableName, IEquatable<FullTableName>
    {
        public FullTableName(string tableName, string schema = null)
        {
            TableName = tableName;
            Schema = schema;
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
            return Comparer.Equals(TableName ?? "", other.TableName ?? "")
                   && Comparer.Equals(Schema ?? "", other.Schema ?? "");
        }

        public override bool Equals(object other)
            => !ReferenceEquals(null, other)
               && other is FullTableName
               && Equals((FullTableName)other);

        public override int GetHashCode()
        {
            unchecked
            {
                return (Comparer.GetHashCode(TableName ?? "") * 397) ^ Comparer.GetHashCode(Schema ?? "");
            }
        }

        public string TableName { get; }
        public string Schema { get; }
        private static readonly IEqualityComparer<string> Comparer = StringComparer.InvariantCultureIgnoreCase;
    }
}