using Xunit;

namespace OoMapper.Tests
{
    public class NotFoundDestinationMembersFacts
    {
        [Fact]
        public void ShouldBeAbleToMap()
        {
            Mapper.Reset();
            Mapper.CreateMap<Source, Destination>();

            Destination destination = null;
            Assert.DoesNotThrow(() => destination = Mapper.Map<Source, Destination>(new Source()));
            Assert.NotNull(destination);
        }

        #region Nested type: Destination

        public class Destination
        {
            public int Property { get; set; }
        }

        #endregion

        #region Nested type: Source

        public class Source
        {
        }

        #endregion
    }

    public class ShouldNotMapCompositePropertyIfFullNameDoesNotMathc
    {
        [Fact]
        public void ShouldNotMap()
        {
            Mapper.Reset();
            Mapper.CreateMap<Source, Destination>();

            var source = new Source
                             {
                                 Other = 100500
                             };

            Destination destination = Mapper.Map<Source, Destination>(source);
            Assert.NotNull(destination);
            Assert.NotEqual("100500", destination.OtherProperty);
        }

        #region Nested type: Destination

        public class Destination
        {
            public string OtherProperty { get; set; }
        }

        #endregion

        #region Nested type: Source

        public class Source
        {
            public int Other { get; set; }
        }

        #endregion
    }
}