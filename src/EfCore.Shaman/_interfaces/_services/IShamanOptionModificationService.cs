namespace EfCore.Shaman
{
    /// <summary>
    ///     Makes modification on ShamanOptions. Can be used for batch changes on ShamanOptions
    /// </summary>
    public interface IShamanOptionModificationService : IShamanService
    {
        void ModifyShamanOptions(ShamanOptions options);
    }
}