namespace EfCore.Shaman
{
    public enum TransactionAction
    {
        /// <summary>
        /// Default action
        /// </summary>
        None,
        /// <summary>
        /// Commit
        /// </summary>
        Commit,
        /// <summary>
        /// Rollback
        /// </summary>
        Rollback
    }
}