namespace EfCore.Shaman
{
    public interface ITableIndexFieldInfo
    {
        #region Properties

        string FieldName { get; }
        bool IsDescending { get; }

        #endregion
    }
}