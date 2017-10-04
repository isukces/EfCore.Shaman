using System;

namespace EfCore.Shaman
{
    /// <summary>
    /// Allows to set default value as SqlValue
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class DefaultValueSqlAttribute : Attribute
    {
        public DefaultValueSqlAttribute(string defaultValueSql)
        {
            DefaultValueSql = defaultValueSql;
        }

        public string DefaultValueSql { get; set; }
    }
}