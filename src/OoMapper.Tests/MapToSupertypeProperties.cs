using Xunit;

namespace OoMapper.Tests
{
    public class MapToSupertypeProperties : TestBase
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
            Assert.Equal(100500, destination.Value);
        }

        [Fact]
        public void TestExplicitMapping()
        {
            Mapper.CreateMap<Source, Destination>()
                .ForMember(x => x.Value, opt => opt.MapFrom(x => x.Value));

            var source = new Source
                             {
                                 Value = 100500,
                             };
            Destination destination = Mapper.Map<Source, Destination>(source);
            Assert.Equal(100500, destination.Value);
        }

        #region Nested type: Destination

        public class Destination : DestinationBase
        {
        }

        #endregion

        #region Nested type: DestinationBase

        public class DestinationBase
        {
            public int Value { get; set; }
        }

        #endregion

        #region Nested type: Source

        public class Source
        {
            public int Value { get; set; }
        }

        #endregion
    }
}