using Xunit;

namespace OoMapper.Tests
{
    public class InheritanceTests : TestBase
    {
        [Fact]
        public void ShouldMapAsBaseClass()
        {
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
            Mapper.CreateMap<ISource, Destination>();
            Destination destination = Mapper.Map<SubSource, Destination>(new SubSource
                                                                             {
                                                                                 Value = "hello world"
                                                                             });
            Assert.Equal("hello world", destination.Value);
        }

        #region Nested type: Destination

        public class Destination
        {
            public string Value { get; set; }
        }

        #endregion

        #region Nested type: ISource

        public interface ISource
        {
            string Value { get; }
        }

        #endregion

        #region Nested type: Source

        public class Source : ISource
        {
            #region ISource Members

            public string Value { get; set; }

            #endregion
        }

        #endregion

        #region Nested type: SubSource

        public class SubSource : Source
        {
        }

        #endregion
    }
}