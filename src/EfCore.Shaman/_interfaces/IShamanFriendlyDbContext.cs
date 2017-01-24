namespace EfCore.Shaman
{
    public interface IShamanFriendlyDbContext
    {
        #region Properties

        DbContextCreationMode CreationMode { get; set; }

        #endregion
    }

    public enum DbContextCreationMode
    {
        Normal,
        WithoutModelFixing
    }
}