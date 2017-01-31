using System.ComponentModel;

namespace EfCore.Shaman.Tests.Model
{
    internal class MyEntityWithDifferentTableName
    {
        #region Properties

        public int Id { get; set; }
        public string Name { get; set; }

        [DefaultValue(11)]
        public int ElevenDefaultValue { get; set; }

        [DefaultValueSql("NONE123")]
        public string NoneDefaultSqlValue { get; set; }
        #endregion
    }
}