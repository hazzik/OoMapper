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
            Destination destination = Mapper.Map<SubSource, Destination>(new SubSource
                                                                             {
                                                                                 Value = "hello world"
                                                                             });
            Assert.Equal("hello world", destination.Value);
        }

        [Fact]
        public void ShouldMapAsInterfaceClass()
        {
            Mapper.Reset();
            Mapper.CreateMap<ISource, Destination>();
            Destination destination = Mapper.Map<SubSource, Destination>(new SubSource
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

        #region Nested type: ISource

        private interface ISource
        {
            string Value { get; }
        }

        #endregion

        #region Nested type: Source

        private class Source : ISource
        {
            #region ISource Members

            public string Value { get; set; }

            #endregion
        }

        #endregion

        #region Nested type: SubSource

        private class SubSource : Source
        {
        }

        #endregion
    }
}