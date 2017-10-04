using System;
using System.Linq;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;

namespace EfCore.Shaman
{
    public static class DbContextExtensions
    {
        public static TResult ComputeInTransaction<T, TResult>(this T context,
            Func<ShamanTransactionInfo<T>, TResult> function)
            where T : DbContext
        {
            var result = default(TResult);
            context.DoInTransaction(ti => { result = function(ti); });
            return result;
        }

        public static void DetachAllModifiedEntities([NotNull] this DbContext context)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));
            var changedEntriesCopy = context.ChangeTracker.Entries()
                .Where(e => e.State == EntityState.Added ||
                            e.State == EntityState.Modified ||
                            e.State == EntityState.Deleted)
                .ToList();
            foreach (var entity in changedEntriesCopy)
                context.Entry(entity.Entity).State = EntityState.Detached;
        }

        public static void DoInTransaction<T>(this T context, Action<ShamanTransactionInfo<T>> action) where T : DbContext
        {
            if (context.Database.CurrentTransaction != null)
            {
                var tmp = new ShamanTransactionInfo<T>(context, false);
                action(tmp);
                return;
            }
            using(var transaction = context.Database.BeginTransaction())
            {
                var transactionFinalize = TransactionAction.Commit;
                try
                {
                    var tmp = new ShamanTransactionInfo<T>(context, true);
                    action(tmp);
                    transactionFinalize = tmp.EndAction;
                }
                catch (Exception e)
                {
                    ShamanOptions.TryGetExceptionLogger(typeof(T))?
                        .LogException(Guid.Parse("{A84D312F-AD7D-4A80-B514-5E4714FD9A9E}"), e);
                    transaction.Rollback();
                    transactionFinalize = TransactionAction.None;
                    throw;
                }
                finally
                {
                    switch (transactionFinalize)
                    {
                        case TransactionAction.Commit:
                            transaction.Commit();
                            break;
                        case TransactionAction.Rollback:
                            transaction.Rollback();
                            break;
                    }
                }
            }
        }
    }
}