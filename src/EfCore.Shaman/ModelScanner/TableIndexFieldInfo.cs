namespace EfCore.Shaman.ModelScanner
{
    internal class TableIndexFieldInfo : ITableIndexFieldInfo
    {
        #region Properties

        public string FieldName { get; set; }
        public bool IsDescending { get; set; }

        #endregion
    }
}