#region using

using System;

#endregion

// ReSharper disable once CheckNamespace

namespace EfCore.Shaman
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Property)]
    public class SqlServerCollationAttribute : Attribute
    {
        #region Constructors

        public SqlServerCollationAttribute()
        {
        }

        public SqlServerCollationAttribute(string collation)
        {
            Collation = collation;
        }

        #endregion

        #region Properties

        public string Collation { get; set; }

        #endregion
    }
}