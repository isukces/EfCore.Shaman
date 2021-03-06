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
            [NotNull] IDiagnosticsLogger<DbLoggerCategory.Database.Transaction> logger)
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

        public override void CommitTransaction()
        {
            CurrentTransaction.Commit();
        }

        public override void RollbackTransaction()
        {
            CurrentTransaction.Rollback();
        }

        public override IDbContextTransaction CurrentTransaction
        {
            get { return _currentTransaction; }
        }

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
           

            
            public Task CommitAsync(CancellationToken cancellationToken = new CancellationToken())
            {
                TransactionManager._currentTransaction = null;
                return Task.CompletedTask;
            }

            public Task RollbackAsync(CancellationToken cancellationToken = new CancellationToken())
            {
                TransactionManager._currentTransaction = null;
                return Task.CompletedTask;
            }
#if EF200
            public Guid TransactionId
                => TransactionManager._currentTransaction?.TransactionId ?? Guid.Empty;
#endif

            private TestInMemoryTransactionManager TransactionManager { get; }
            public ValueTask DisposeAsync()
            {
                TransactionManager._currentTransaction = null;
                return new ValueTask(Task.CompletedTask);
            }
        }
    }
}