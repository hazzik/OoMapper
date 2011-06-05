using Xunit;

namespace OoMapper.Tests
{
    public class MapTopLevelPrimitives
    {
        [Fact]
        public void MapIntToInt()
        {
            int destination = Mapper.Map<int, int>(1);
            Assert.Equal(1, destination);
        }

        [Fact]
        public void MapIntToLong()
        {
            long destination = Mapper.Map<int, long>(1);
            Assert.Equal(1, destination);
        }

        [Fact]
        public void MapIntToString()
        {
            string destination = Mapper.Map<int, string>(1);
            Assert.Equal("1", destination);
        }
    }
}