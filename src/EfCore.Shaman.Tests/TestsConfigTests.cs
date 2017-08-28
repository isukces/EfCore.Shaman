using Xunit;

namespace EfCore.Shaman.Tests
{
    public class TestsConfigTests
    {

        [Fact]
        public void T01_ShouldReadTestsConfig()
        {
            var cfg = TestsConfig.Load();
            Assert.NotNull(cfg);
            Assert.False(string.IsNullOrEmpty(cfg.ConnectionStringTemplate));
        }

    }
}