using System;
using Xunit;

namespace OoMapper.Tests
{
    public class CustomPropertyMappingFacts
    {
        [Fact]
        public void NewFact()
        {
            Mapper.CreateMap<Source, Destination>()
                .ForMember(x => x.SomeProperty, opt => opt.MapFrom(x => x.OtherProperty))
                .ForMember(x => x.Child, opt => opt.Ignore());

            var source = new Source
                             {
                                 OtherProperty = "hello",
                             };
            Destination destination = Mapper.Map<Source, Destination>(source);
            Assert.Equal("hello", destination.SomeProperty);
        }

        [Fact]
        public void MapWithDifferentSourceType()
        {
            Mapper.CreateMap<SourceChild, DestinationChild>();
            Mapper.CreateMap<Source, Destination>()
                .ForMember(x => x.SomeProperty, opt => opt.Ignore())
                .ForMember(x => x.Child, opt => opt.MapFrom(x => x.ChildProperty));

            var source = new Source
                             {
                                 ChildProperty = new SourceChild
                                                     {
                                                         A = "X"
                                                     },
                             };
            Destination destination = Mapper.Map<Source, Destination>(source);
            Assert.Equal("X", destination.Child.A);
        }

        #region Nested type: Destination

        private class Destination
        {
            public string SomeProperty { get; set; }

            public DestinationChild Child { get; set; }
        }

        private class DestinationChild
        {
            public string A { get; set; }
        }

        private class SourceChild
        {
            public string A { get; set; }
        }

        #endregion

        #region Nested type: Source

        private class Source
        {
            public string OtherProperty { get; set; }
            public SourceChild ChildProperty { get; set; }
        }

        #endregion
    }
}