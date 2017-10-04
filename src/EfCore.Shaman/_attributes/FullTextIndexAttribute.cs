namespace EfCore.Shaman
{
    public sealed class FullTextIndexAttribute : AbstractIndexAttribute
    {
        public FullTextIndexAttribute(string name = null, int order = 0, bool isDescending = false)
            : base(name, order, isDescending)
        {
        }

        public string FullTextCatalogName { get; set; }
    }
}