using Xunit;

namespace OoMapper.Tests
{
    public class InheritanceTests
    {
        [Fact]
        public void ShouldMapAsBaseClass()
        {
            Mapper.Reset();
            Mapper.CreateMap<Source, Destination>();
            var destination = Mapper.Map<SubSource, Destination>(new SubSource
                                                                     {
                                                                         Value = "hello world"
                                                                     });
            Assert.Equal("hello world", destination.Value);
        }

        #region Nested type: Destination

        private class Destination
        {
            public string Value { get; set; }
        }

        #endregion

        #region Nested type: Source

        private class Source
        {
            public string Value { get; set; }
        }

        #endregion

        #region Nested type: SubSource

        private class SubSource : Source
        {
        }

        #endregion
    }
}