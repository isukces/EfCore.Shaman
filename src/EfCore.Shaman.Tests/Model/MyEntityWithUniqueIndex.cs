using EfCore.Shaman;

namespace EfCore.Shaman.Tests.Model
{
    internal class MyEntityWithUniqueIndex
    {
        #region Properties

        public int Id { get; set; }

        [UniqueIndex]
        public string Name { get; set; }

        #endregion
    }
}