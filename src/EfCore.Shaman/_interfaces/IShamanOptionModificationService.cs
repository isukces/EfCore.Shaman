namespace EfCore.Shaman
{
    /// <summary>
    ///     Makes modification on ShamanOptions. Can be used for batch changes on ShamanOptions
    /// </summary>
    public interface IShamanOptionModificationService : IShamanService
    {
        #region Instance Methods

        void ModifyShamanOptions(ShamanOptions options);

        #endregion
    }
}