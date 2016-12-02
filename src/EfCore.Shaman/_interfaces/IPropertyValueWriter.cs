namespace EfCore.Shaman
{
    public interface IPropertyValueWriter
    {
        #region Instance Methods

        void WritePropertyValue(object obj, object value);

        #endregion
    }
}