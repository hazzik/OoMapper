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
}