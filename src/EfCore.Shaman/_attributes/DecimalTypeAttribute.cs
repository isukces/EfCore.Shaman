#region using

using System;

#endregion

namespace EfCore.Shaman
{
    [AttributeUsage(AttributeTargets.Property)]
    public class DecimalTypeAttribute : Attribute
    {
        #region Constructors

        public DecimalTypeAttribute(int length, int decimalPlaces)
        {
            Length = length;
            DecimalPlaces = decimalPlaces;
        }

        #endregion

        #region Properties

        public int Length { get; set; }
        public int DecimalPlaces { get; set; }

        #endregion
    }
}