using System.Collections.Generic;
using Xunit;

namespace OoMapper.Tests
{
    public class MapCollectionIgnoreMapperOrder
    {
        [Fact]
        public void Test()
        {
            Mapper.CreateMap<Source, Destination>();
            Mapper.CreateMap<SourceChild, DestinationChild>();

            var source = new Source
                             {
                                 Children = new[]
                                                {
                                                    new SourceChild
                                                        {
                                                            A = "hello world"
                                                        },
                                                }
                             };

            Destination destination = Mapper.Map<Source, Destination>(source);
            Assert.Equal(1, destination.Children.Length);
        }

        #region Nested type: Destination

        public class Destination
        {
            public DestinationChild[] Children { get; set; }
        }

        #endregion

        #region Nested type: DestinationChild

        public class DestinationChild
        {
            public string A { get; set; }
        }

        #endregion

        #region Nested type: Source

        public class Source
        {
            public IEnumerable<SourceChild> Children { get; set; }
        }

        #endregion

        #region Nested type: SourceChild

        public class SourceChild
        {
            public string A { get; set; }
        }

        #endregion
    }
}