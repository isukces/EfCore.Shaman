#region using

using System;

#endregion

namespace EfCore.Shaman
{
    public abstract class AbstractIndexAttribute : Attribute
    {
        #region Constructors

        public AbstractIndexAttribute(string name = OneFieldIndexWithAutoName, int order = 0, bool isDescending = false)
        {
            Name = name;
            Order = order;
            IsDescending = isDescending;
        }

        #endregion

        #region Properties

        public string Name { get; set; }
        public int Order { get; set; }
        public bool IsDescending { get; set; }
      

        #endregion

        #region Other

        public const string OneFieldIndexWithAutoName = null;

        #endregion
    }
}