using Xunit;

namespace OoMapper.Tests
{
    public class MapFromSupertypeProperties
    {
        [Fact]
        public void TestAutomapping()
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

        private class Destination
        {
            public int Value { get; set; }
        }

        #endregion

        #region Nested type: Source

        private class Source : SourceParent
        {
        }

        #endregion

        #region Nested type: SourceParent

        private class SourceParent
        {
            public int Value { get; set; }
        }

        #endregion
    }

    public class MapToSupertypeProperties
    {
        [Fact]
        public void Test()
        {
            Mapper.Reset();
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
            Mapper.Reset();
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

        private class Destination : DestinationBase
        {
        }

        #endregion

        #region Nested type: DestinationBase

        private class DestinationBase
        {
            public int Value { get; set; }
        }

        #endregion

        #region Nested type: Source

        private class Source
        {
            public int Value { get; set; }
        }

        #endregion
    }
}