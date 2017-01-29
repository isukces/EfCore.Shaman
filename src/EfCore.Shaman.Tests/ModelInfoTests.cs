#region using

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EfCore.Shaman.ModelScanner;
using EfCore.Shaman.Tests.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.Storage.Internal;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Xunit;

#endregion

namespace EfCore.Shaman.Tests
{
    public class ModelInfoTests
    {
        #region Static Methods

        private static void DoTestOnModelBuilder<T>(Action<ModelBuilder> checkMethod) where T : VisitableDbContext
        {
            var options = new DbContextOptionsBuilder<T>()
                .UseInMemoryDatabase(nameof(T02_ShouldHaveUniqueIndex))
                .Options;
            var count = 0;
            using(var context = InstanceCreator.CreateInstance<T>(options))
            {
                context.ExternalCheckModel = b =>
                {
                    count++;
                    checkMethod?.Invoke(b);
                };
                var tmp = context.Settings.ToArray(); // enforce to create model
                var model = context.Model;
                if (model == null) // enforce to create model
                    throw new NullReferenceException();
            }
            if (count == 0)
                throw new Exception("checkMethod has not been invoked");
        }

        private static ModelInfo GetModelInfo<T>(IList<IShamanService> services = null)
        {
            var aa = new ModelInfo(typeof(T), services);
            return aa;
        }

        private static string SerializeToTest(object data)
        {
            return data == null ? "null" : JsonConvert.SerializeObject(data).Replace("\"", "'");
        }

        #endregion

        #region Instance Methods

        [Fact]
        public void T01_ShouldRecoginzeNotNull()
        {
            Assert.True(ModelInfo.NotNullFromPropertyType(typeof(int)));
            Assert.True(ModelInfo.NotNullFromPropertyType(typeof(long)));
            Assert.True(ModelInfo.NotNullFromPropertyType(typeof(Guid)));

            Assert.False(ModelInfo.NotNullFromPropertyType(typeof(int?)));
            Assert.False(ModelInfo.NotNullFromPropertyType(typeof(long?)));
            Assert.False(ModelInfo.NotNullFromPropertyType(typeof(Guid?)));

            Assert.False(ModelInfo.NotNullFromPropertyType(typeof(string)));
        }

        [Fact]
        public void T02_ShouldHaveUniqueIndex()
        {
            // todo: xunit tests (each test in separate appdomain). DbContext creates Model only once  
            DoTestOnModelBuilder<TestDbContext>(mb =>
            {
                var modelInfo = GetModelInfo<TestDbContext>();
                var dbSet = modelInfo.DbSet<MyEntityWithUniqueIndex>();
                Assert.NotNull(dbSet);
                var idxs = SerializeToTest(dbSet.Indexes);
                Assert.Equal(
                    "[{'IndexName':'','Fields':[{'FieldName':'Name','IsDescending':false}],'IsUnique':true}]",
                    idxs);
            });
        }

        [Fact]
        public void T03_ShouldHaveManuallyChangedTableName()
        {
            DoTestOnModelBuilder<TestDbContext>(mb =>
            {
                var modelInfo = GetModelInfo<TestDbContext>();
                var dbSet = modelInfo.DbSet<MyEntityWithDifferentTableName>();
                Assert.NotNull(dbSet);
                Assert.Equal("ManualChange", dbSet.TableName);
            });
        }


        [Fact]
        public void T04_ShouldHaveEmptyService()
        {
            var services = ShamanOptions.CreateShamanOptions(typeof(TestDbContext)).Services;
            var cnt = services.Count(a => a is EmptyService);
            Assert.Equal(1, cnt);
        }

        [Fact]
        public void T05_ShouldHaveDefaultSchema()
        {
            var mi = ModelInfo.Make<TestDbContext>();
            Assert.Equal("testSchema", mi.DefaultSchema);
        }

        [Fact]
        public void T06_ShouldHaveTableNameWithPrefix()
        {
            const string expectedTableName = "myPrefixUser";

            DoTestOnModelBuilder<PrefixedTableNamesDbContext>(mb =>
            {
                var t = mb.Model.GetEntityTypes().Single(a => a.ClrType == typeof(User));
                Assert.NotNull(t);
                Assert.Equal(expectedTableName, t.Relational().TableName);

                // without patching
                {
                    var modelInfo = GetModelInfo<PrefixedTableNamesDbContext>(ShamanOptions.Default.Services);
                    var dbSet = modelInfo.DbSet<User>();
                    Assert.NotNull(dbSet);
                    Assert.Equal("User", dbSet.TableName);
                }
                // with patching
                {
                    var modelInfo = GetModelInfo<PrefixedTableNamesDbContext>();
                    var dbSet = modelInfo.DbSet<User>();
                    Assert.NotNull(dbSet);
                    Assert.Equal(expectedTableName, dbSet.TableName);
                }
            });

            {
                var mi = ModelInfo.Make<PrefixedTableNamesDbContext>();
                var dbSet = mi.DbSets.Single(a => a.EntityType == typeof(User));
                Assert.Equal(expectedTableName, dbSet.TableName);
            }
        }

        #endregion
    }


    public class TestInMemoryTransactionManager : InMemoryTransactionManager
    {
        private IDbContextTransaction _currentTransaction;

        public TestInMemoryTransactionManager(ILogger<InMemoryTransactionManager> logger)
            : base(logger)
        {
        }

        public override IDbContextTransaction CurrentTransaction => _currentTransaction;

        public override IDbContextTransaction BeginTransaction()
        {
            _currentTransaction = new TestInMemoryTransaction(this);
            return _currentTransaction;
        }

        public override Task<IDbContextTransaction> BeginTransactionAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            _currentTransaction = new TestInMemoryTransaction(this);
            return Task.FromResult(_currentTransaction);
        }

        public override void CommitTransaction() => CurrentTransaction.Commit();

        public override void RollbackTransaction() => CurrentTransaction.Rollback();

        private class TestInMemoryTransaction : IDbContextTransaction
        {
            public TestInMemoryTransaction(TestInMemoryTransactionManager transactionManager)
            {
                TransactionManager = transactionManager;
            }

            private TestInMemoryTransactionManager TransactionManager { get; }

            public void Dispose()
            {
                TransactionManager._currentTransaction = null;
            }

            public void Commit()
            {
                TransactionManager._currentTransaction = null;
            }

            public void Rollback()
            {
                TransactionManager._currentTransaction = null;
            }
        }
    }
}