namespace EfCore.Shaman
{
    public sealed class IndexAttribute : AbstractIndexAttribute
    {
        #region Constructors

        public IndexAttribute(string name = OneFieldIndexWithAutoName, int order = 0, bool isDescending = false)
            : base(name, order, isDescending)
        {
        }

        #endregion
    }
}