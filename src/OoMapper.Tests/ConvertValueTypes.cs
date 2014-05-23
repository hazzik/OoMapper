using Xunit;

namespace OoMapper.Tests
{
	public class ConvertValueTypes
    {
        [Fact]
        public void Test()
        {
            Mapper.CreateMap<Source, Destination>();
            var source = new Source
                             {
                                 Value1 = 100500,
                                 Value2 = 500100,
                                 Value3 = 42,
                             };
            Destination destination = Mapper.Map<Source, Destination>(source);
            Assert.Equal(100500, destination.Value1);
            Assert.Equal(500100, destination.Value2);
            Assert.Equal(42, destination.Value3);
        }

	    public class Destination
        {
            public long Value1 { get; set; }

            public int Value2 { get; set; }

            public int Value3 { get; set; }
        }

	    public class Source
        {
            public int Value1 { get; set; }

            public long Value2 { get; set; }

	        public byte Value3 { get; set; }
        }
    }
}