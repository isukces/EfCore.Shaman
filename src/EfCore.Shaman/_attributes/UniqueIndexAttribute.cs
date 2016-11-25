namespace EfCore.Shaman
{
    public sealed class UniqueIndexAttribute : AbstractIndexAttribute
    {
        #region Constructors

        public UniqueIndexAttribute(string name = OneFieldIndexWithAutoName, int order = 0, bool isDescending = false)
            : base(name, order, isDescending)
        {
        }

        #endregion
    }
}