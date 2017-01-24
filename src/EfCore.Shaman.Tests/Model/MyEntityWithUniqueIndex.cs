namespace EfCore.Shaman.Tests.Model
{
    internal class MyEntityWithUniqueIndex
    {
        public int Id { get; set; }

        [UniqueIndex]
        public string Name { get; set; }
    }
}