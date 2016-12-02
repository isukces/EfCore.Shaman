namespace EfCore.Shaman
{
    public interface IPropertyValueReader
    {
        #region Instance Methods

        object ReadPropertyValue(object obj);

        #endregion
    }
}