#region using

using System;

#endregion

namespace EfCore.Shaman
{
    [AttributeUsage(AttributeTargets.Property)]
    public class DefaultValueSqlAttribute : Attribute
    {
        #region Constructors

        public DefaultValueSqlAttribute(string valueSql)
        {
            ValueSql = valueSql;
        }

        #endregion

        #region Properties

        public string ValueSql { get; set; }

        #endregion
    }
}