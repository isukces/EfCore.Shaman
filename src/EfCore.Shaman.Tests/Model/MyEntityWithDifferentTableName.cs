namespace EfCore.Shaman.Tests.Model
{
    internal class MyEntityWithDifferentTableName
    {
        #region Properties

        public int Id { get; set; }
        public string Name { get; set; }

        [DefaultValue(11)]
        public int ElevenDefaultValue { get; set; }
        #endregion
    }
}