using Xunit;

namespace OoMapper.Tests
{
    public class MapObjectToString
    {
        [Fact]
        public void Test()
        {
            Mapper.CreateMap<Source, Destination>();
            var source = new Source
                             {
                                 Value = 100500,
                             };
            Destination destination = Mapper.Map<Source, Destination>(source);
            Assert.Equal("100500", destination.Value);
        }

        public class Destination
        {
            public string Value { get; set; }
        }

        public class Source
        {
            public int Value { get; set; }
        }
    }
}