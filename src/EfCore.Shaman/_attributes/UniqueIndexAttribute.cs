using System;

namespace EfCore.Shaman
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = true)]
    public sealed class UniqueIndexAttribute : AbstractIndexAttribute
    {
        public UniqueIndexAttribute(string name = OneFieldIndexWithAutoName, int order = 0, bool isDescending = false)
            : base(name, order, isDescending)
        {
        }
    }
}