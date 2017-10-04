namespace EfCore.Shaman
{
    public interface IPropertyValueReader
    {
        object ReadPropertyValue(object obj);
    }
}