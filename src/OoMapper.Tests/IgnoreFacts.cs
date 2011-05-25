using Xunit;

namespace OoMapper.Tests
{
    public class IgnoreFacts
    {
        [Fact]
        public void NewFact()
        {
            Mapper.CreateMap<Source, Destination>()
                .ForMember(x => x.IgnoredProperty, opt => opt.Ignore());

            var source = new Source
                             {
                                 SomeProperty = "hello",
                                 IgnoredProperty = "world",
                             };
            Destination destination = Mapper.Map<Source, Destination>(source);
            Assert.Equal("hello", destination.SomeProperty);
            Assert.Null(destination.IgnoredProperty);
        }

        #region Nested type: Destination

        private class Destination
        {
            public string SomeProperty { get; set; }
            public string IgnoredProperty { get; set; }
        }

        #endregion

        #region Nested type: Source

        private class Source
        {
            public string SomeProperty { get; set; }
            public string IgnoredProperty { get; set; }
        }

        #endregion
    }
}