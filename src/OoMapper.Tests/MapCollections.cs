using System;
using Xunit;

namespace OoMapper.Tests
{
    public class MapCollections
    {
        [Fact]
        public void NewFact()
        {
            Mapper.Configure<SourceChild, DestinationChild>();
            Mapper.Configure<Source, Destination>();

            Destination destination = Mapper.Map<Source, Destination>(new Source
                                                                          {
                                                                              Children = new[]
                                                                                             {
                                                                                                 new SourceChild
                                                                                                     {
                                                                                                         A = "hello world"
                                                                                                     },
                                                                                             }
                                                                          });
            Assert.Equal(1, destination.Children.Length);
        }

        #region Nested type: Destination

        private class Destination
        {
            public DestinationChild[] Children { get; set; }
        }

        #endregion

        #region Nested type: DestinationChild

        private class DestinationChild
        {
            public string A { get; set; }
        }

        #endregion

        #region Nested type: Source

        private class Source
        {
            public SourceChild[] Children { get; set; }
        }

        #endregion

        #region Nested type: SourceChild

        private class SourceChild
        {
            public string A { get; set; }
        }

        #endregion
    }
}
