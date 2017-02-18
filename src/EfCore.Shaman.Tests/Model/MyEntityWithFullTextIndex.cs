namespace EfCore.Shaman.Tests.Model
{
    internal class MyEntityWithFullTextIndex
    {
        #region Properties

        public int Id { get; set; }

        [FullTextIndex()]
        public string Name { get; set; }

        #endregion
    }
}