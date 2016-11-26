using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EfCore.Shaman.ModelScanner;
using Xunit;

namespace EfCore.Shaman.Tests
{
    public class ModelInfoTests
    {
        [Fact]
        public void ShouldRecoginzeNotNull()
        {
            Assert.True(ModelInfo.NotNullFromPropertyType(typeof(int)));
            Assert.True(ModelInfo.NotNullFromPropertyType(typeof(long)));
            Assert.True(ModelInfo.NotNullFromPropertyType(typeof(Guid)));

            Assert.False(ModelInfo.NotNullFromPropertyType(typeof(int?)));
            Assert.False(ModelInfo.NotNullFromPropertyType(typeof(long?)));
            Assert.False(ModelInfo.NotNullFromPropertyType(typeof(Guid?)));

            Assert.False(ModelInfo.NotNullFromPropertyType(typeof(string)));

        }
    }
}
