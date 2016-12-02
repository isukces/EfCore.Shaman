namespace EfCore.Shaman
{
    public interface IFullTableName
    {
        string TableName { get; }
        string Schema { get; }
    }
}
