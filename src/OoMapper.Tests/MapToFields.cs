using Xunit;

namespace OoMapper.Tests
{
    public class MapToFields : TestBase
    {
        [Fact]
        public void MapToField()
        {
            Mapper.CreateMap<Source, Destination>();

            var source = new Source
                             {
                                 Field = 123
                             };
            Destination destination = Mapper.Map<Source, Destination>(source);
            Assert.Equal(123, destination.Field);
        }

        [Fact]
        public void ShouldNotMapToStaticField()
        {
            Mapper.CreateMap<Source, Destination>();

            var source = new Source
                             {
                                 Field = 123,
                                 StaticField = 500
                             };
            Mapper.Map<Source, Destination>(source);
            Assert.NotEqual(500, Destination.StaticField);
        }

        #region Nested type: Destination

        public class Destination
        {
            public static int StaticField;
            public int Field;
        }

        #endregion

        #region Nested type: Source

        public class Source
        {
            public int Field { get; set; }
            public int StaticField { get; set; }
        }

        #endregion
    }
}