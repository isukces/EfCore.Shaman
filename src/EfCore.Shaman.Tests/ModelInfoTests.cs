#region using

using System;
using System.Linq;
using EfCore.Shaman.ModelScanner;
using EfCore.Shaman.Tests.Model;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Xunit;

#endregion

namespace EfCore.Shaman.Tests
{
    public class ModelInfoTests
    {
        #region Static Methods

        private static void DoTestOnModelBuilder(Action<ModelBuilder> checkMethod)
        {
            var options = new DbContextOptionsBuilder<TestDbContext>()
                .UseInMemoryDatabase(nameof(T02_ShouldHaveUniqueIndex))
                .Options;

            using(var context = new TestDbContext(options))
            {
                context.ExternalCheckModel = checkMethod;
                var list = context.EntityWithUniqueIndex.ToArray(); // force build model
            }
        }

        private static ModelInfo GetModelInfo()
        {
            var aa = new ModelInfo(typeof(TestDbContext), ShamanOptions.Default.Services);
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
            DoTestOnModelBuilder(mb =>
            {
                var modelInfo = GetModelInfo();
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
            DoTestOnModelBuilder(mb =>
            {
                var modelInfo = GetModelInfo();
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

        #endregion
    }
}