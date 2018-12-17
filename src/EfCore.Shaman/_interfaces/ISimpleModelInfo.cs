namespace EfCore.Shaman
{
    public interface ISimpleModelInfo:IShamanAnnotatable 
    {
        string DefaultSchema { get; }

        bool DefaultIsUnicodeText { get; }
    }
    
    public interface IUpdatableSimpleModelInfo : IShamanAnnotatable
    {
        string DefaultSchema { get; set; }

        bool DefaultIsUnicodeText { get; set; }
    }
}