using System;

namespace EfCore.Shaman
{
    [AttributeUsage(AttributeTargets.Property)]
    public class DecimalTypeAttribute : Attribute
    {
        public DecimalTypeAttribute(int length, int decimalPlaces)
        {
            Length = length;
            DecimalPlaces = decimalPlaces;
        }

        public int Length { get; set; }
        public int DecimalPlaces { get; set; }
    }
}