namespace EfCore.Shaman
{
    public interface ISimpleModelInfo
    {
        string DefaultSchema { get; }

        bool DefaultIsUnicodeText { get; }
    }
}