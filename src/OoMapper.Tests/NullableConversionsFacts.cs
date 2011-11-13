using Xunit;

namespace OoMapper.Tests
{
    public class NullableConversionsFacts : TestBase
    {
        [Fact]
        public void ShouldBeAbleToMapValueTypeToItsNullable()
        {
            Mapper.CreateMap<Source, Destination>();
            var source = new Source
                             {
                                 Property = 100
                             };
            Destination destination = Mapper.Map<Source, Destination>(source);
            Assert.Equal(100, destination.Property);
        }

        [Fact]
        public void ShouldBeAbleToMapNullableToItsNotNullable()
        {
            Mapper.CreateMap<Destination, Source>();
            var source = new Destination
                             {
                                 Property = 100,
                             };
            Source destination = Mapper.Map<Destination, Source>(source);
            Assert.Equal(100, destination.Property);
        }

        [Fact]
        public void ShouldBeAbleToMapNullableNullToItsNotNullable()
        {
            Mapper.CreateMap<Destination, Source>();
            var source = new Destination();
            Source destination = Mapper.Map<Destination, Source>(source);
            Assert.Equal(0, destination.Property);
        }

        #region Nested type: Destination

        public class Destination
        {
            public int? Property { get; set; }
        }

        #endregion

        #region Nested type: Source

        public class Source
        {
            public int Property { get; set; }
        }

        #endregion
    }
}