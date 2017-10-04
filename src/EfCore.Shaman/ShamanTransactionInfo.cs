using Microsoft.EntityFrameworkCore;

namespace EfCore.Shaman
{
    public class ShamanTransactionInfo<T> where T : DbContext
    {
        public ShamanTransactionInfo(T context, bool ownTransaction)
        {
            OwnTransaction = ownTransaction;
            Context = context;
        }

        public bool OwnTransaction { get; }
        public T Context { get; }

        /// <summary>
        ///     Transaction commit, rollback or default 
        /// </summary>
        public TransactionAction EndAction { get; set; } = TransactionAction.Commit;
    }
}