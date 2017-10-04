namespace EfCore.Shaman
{
    public interface IPropertyValueWriter
    {
        void WritePropertyValue(object obj, object value);
    }
}