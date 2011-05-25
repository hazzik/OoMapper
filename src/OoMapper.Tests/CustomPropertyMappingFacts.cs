using Xunit;

namespace OoMapper.Tests
{
    public class CustomPropertyMappingFacts
    {
        [Fact]
        public void NewFact()
        {
            Mapper.CreateMap<Source, Destination>()
                .ForMember(x => x.SomeProperty, opt => opt.MapFrom(x => x.OtherProperty));
            int i = sizeof (float);

            var source = new Source
                             {
                                 OtherProperty = "hello",
                             };
            Destination destination = Mapper.Map<Source, Destination>(source);
            Assert.Equal("hello", destination.SomeProperty);
        }

        #region Nested type: Destination

        private class Destination
        {
            public string SomeProperty { get; set; }
        }

        #endregion

        #region Nested type: Source

        private class Source
        {
            public string OtherProperty { get; set; }
        }

        #endregion
    }
}