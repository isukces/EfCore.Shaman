#region using

using System;

#endregion

namespace EfCore.Shaman
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class DefaultValueAttribute : Attribute
    {
        #region Constructors

        public DefaultValueAttribute(object defaultValue)
        {
            DefaultValue = defaultValue;
        }

        #endregion

        #region Properties

        public object DefaultValue { get; set; }

        #endregion
    }
}