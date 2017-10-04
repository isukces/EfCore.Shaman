namespace EfCore.Shaman
{
    public interface ITableIndexFieldInfo
    {
        string FieldName { get; }
        bool IsDescending { get; }
    }
}