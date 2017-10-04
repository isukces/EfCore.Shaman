using System;

namespace EfCore.Shaman
{
    [AttributeUsage(AttributeTargets.Property)]
    public class NavigationPropertyAttribute : Attribute
    {
        public NavigationPropertyAttribute(bool forceNavigation = true)
        {
            ForceNavigation = forceNavigation;
        }

        public bool ForceNavigation { get; set; }
    }
}