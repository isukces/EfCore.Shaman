namespace EfCore.Shaman
{
    public sealed class FullTextIndexAttribute : AbstractIndexAttribute
    {
        #region Constructors

        public FullTextIndexAttribute(string name = null, int order = 0, bool isDescending = false)
            : base(name, order, isDescending)
        {
        }

        #endregion

        #region Properties

        public string FullTextCatalogName { get; set; }

        #endregion
    }
}