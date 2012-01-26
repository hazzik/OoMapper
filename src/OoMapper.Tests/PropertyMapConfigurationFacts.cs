using Xunit;

namespace OoMapper.Tests
{
    public class PropertyMapConfigurationFacts
    {
        [Fact]
        public void IgnoredIsMapped()
        {
            var pmc = new PropertyMapConfiguration(mi => true, 1);
            pmc.Ignore();
            Assert.True(pmc.IsMapped());
        }

        [Fact]
        public void IgnoredIsIgnored()
        {
            var pmc = new PropertyMapConfiguration(mi => true, 1);
            pmc.Ignore();
            Assert.True(pmc.IsIgnored());
        }

        [Fact]
        public void WithCustomResolverIsMapped()
        {
            var pmc = new PropertyMapConfiguration(mi => true, 1);
            pmc.SetCustomResolver(new LambdaSourceMemberResolver(null));
            Assert.True(pmc.IsMapped());
        }
    }
}