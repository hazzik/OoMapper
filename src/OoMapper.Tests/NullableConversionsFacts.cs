using Xunit;

namespace OoMapper.Tests
{
    public class NullableConversionsFacts
    {
        [Fact]
        public void ShouldBeAbleToMapValueTypeToItsNullable()
        {
            Mapper.Reset();
            Mapper.CreateMap<Source, Destination>();
            var source = new Source
                             {
                                 Property = 100
                             };
            var destination = Mapper.Map<Source, Destination>(source);
            Assert.Equal(100, destination.Property);
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