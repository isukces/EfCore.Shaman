#region using

using System;

#endregion

namespace EfCore.Shaman
{
    [AttributeUsage(AttributeTargets.Property)]
    public class NavigationPropertyAttribute : Attribute
    {
        #region Constructors

        public NavigationPropertyAttribute(bool forceNavigation = true)
        {
            ForceNavigation = forceNavigation;
        }

        #endregion

        #region Properties

        public bool ForceNavigation { get; set; }

        #endregion
    }
}