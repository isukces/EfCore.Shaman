namespace EfCore.Shaman.ModelScanner
{
    public class ColumnIndexInfo
    {
        public string IndexName { get; set; }
        public int Order { get; set; }
        public bool IsDescending { get; set; }

        public bool IsUnique { get; set; }
    }
}