#region using

using System;

#endregion

namespace EfCore.Shaman
{
    /// <summary>
    /// Allows to set default value as SqlValue
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class DefaultValueSqlAttribute : Attribute
    {
        #region Constructors

        public DefaultValueSqlAttribute(string defaultValueSql)
        {
            DefaultValueSql = defaultValueSql;
        }

        #endregion

        #region Properties

        public string DefaultValueSql { get; set; }

        #endregion
    }
}