using System;

namespace EfCore.Shaman
{
    public abstract class AbstractIndexAttribute : Attribute
    {
        public AbstractIndexAttribute(string name = OneFieldIndexWithAutoName, int order = 0, bool isDescending = false)
        {
            Name = name;
            Order = order;
            IsDescending = isDescending;
        }

        public string Name { get; set; }
        public int Order { get; set; }
        public bool IsDescending { get; set; }


        public const string OneFieldIndexWithAutoName = null;
    }
}