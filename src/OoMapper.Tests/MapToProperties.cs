using Xunit;

namespace OoMapper.Tests
{
    public class MapToProperties : TestBase
    {
        [Fact]
        public void MapToField()
        {
            Mapper.CreateMap<Source, Destination>();

            var source = new Source
                             {
                                 Property = 123
                             };
            Destination destination = Mapper.Map<Source, Destination>(source);
            Assert.Equal(123, destination.Property);
        }

        [Fact]
        public void ShouldNotMapToStaticField()
        {
            Mapper.CreateMap<Source, Destination>();

            var source = new Source
                             {
                                 Property = 123,
                                 StaticProperty = 500
                             };
            Mapper.Map<Source, Destination>(source);
            Assert.NotEqual(500, Destination.StaticProperty);
        }

        #region Nested type: Destination

        public class Destination
        {
            public static int StaticProperty { get; set; }
            public int Property { get; set; }
        }

        #endregion

        #region Nested type: Source

        public class Source
        {
            public int Property { get; set; }
            public int StaticProperty { get; set; }
        }

        #endregion
    }
}