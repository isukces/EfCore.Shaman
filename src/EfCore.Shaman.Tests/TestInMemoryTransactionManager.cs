using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.Storage.Internal;
using Microsoft.Extensions.Logging;

namespace EfCore.Shaman.Tests
{
    public class TestInMemoryTransactionManager : InMemoryTransactionManager
    {
        #region Constructors

        public TestInMemoryTransactionManager(ILogger<InMemoryTransactionManager> logger)
            : base(logger)
        {
        }

        #endregion

        #region Instance Methods

        public override IDbContextTransaction BeginTransaction()
        {
            _currentTransaction = new TestInMemoryTransaction(this);
            return _currentTransaction;
        }

        public override Task<IDbContextTransaction> BeginTransactionAsync(
            CancellationToken cancellationToken = default(CancellationToken))
        {
            _currentTransaction = new TestInMemoryTransaction(this);
            return Task.FromResult(_currentTransaction);
        }

        public override void CommitTransaction() => CurrentTransaction.Commit();

        public override void RollbackTransaction() => CurrentTransaction.Rollback();

        #endregion

        #region Properties

        public override IDbContextTransaction CurrentTransaction => _currentTransaction;

        #endregion

        #region Fields

        private IDbContextTransaction _currentTransaction;

        #endregion

        #region Nested

        private class TestInMemoryTransaction : IDbContextTransaction
        {
            #region Constructors

            public TestInMemoryTransaction(TestInMemoryTransactionManager transactionManager)
            {
                TransactionManager = transactionManager;
            }

            #endregion

            #region Instance Methods

            public void Commit()
            {
                TransactionManager._currentTransaction = null;
            }

            public void Dispose()
            {
                TransactionManager._currentTransaction = null;
            }

            public void Rollback()
            {
                TransactionManager._currentTransaction = null;
            }

            #endregion

            #region Properties

            private TestInMemoryTransactionManager TransactionManager { get; }

            #endregion
        }

        #endregion
    }
}