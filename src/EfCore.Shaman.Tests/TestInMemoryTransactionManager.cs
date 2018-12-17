using System;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.InMemory.Storage.Internal;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.Storage.Internal;

namespace EfCore.Shaman.Tests
{
    public class TestInMemoryTransactionManager : InMemoryTransactionManager
    {
#if EF200
        public TestInMemoryTransactionManager(
            [CanBeNull] IDiagnosticsLogger<DbLoggerCategory.Database.Transaction> logger)
            : base(logger)
        {
        }
#else
        public TestInMemoryTransactionManager([CanBeNull] ILogger<InMemoryTransactionManager> logger)
            : base(logger)
        {
        }
       
#endif

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

        public override IDbContextTransaction CurrentTransaction => _currentTransaction;

        private IDbContextTransaction _currentTransaction;

        private class TestInMemoryTransaction : IDbContextTransaction
        {
            public TestInMemoryTransaction(TestInMemoryTransactionManager transactionManager)
            {
                TransactionManager = transactionManager;
            }

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
#if EF200
            public Guid TransactionId
                => TransactionManager._currentTransaction?.TransactionId ?? Guid.Empty;
#endif

            private TestInMemoryTransactionManager TransactionManager { get; }
        }
    }
}