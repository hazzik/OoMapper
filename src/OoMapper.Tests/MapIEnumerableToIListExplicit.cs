using System.Collections.Generic;
using Xunit;

namespace OoMapper.Tests
{
    public class MapIEnumerableToIListExplicit
    {
        [Fact]
        public void Test()
        {
            Mapper.CreateMap<SourceChild, DestinationChild>();
            Mapper.CreateMap<Source, Destination>()
                .ForMember(x => x.Children, opt => opt.MapFrom(x => x.Children));

            var source = new Source
                             {
                                 Children = new[]
                                                {
                                                    new SourceChild
                                                        {
                                                            A = "hello world"
                                                        }
                                                }
                             };

            Destination destination = Mapper.Map<Source, Destination>(source);
            Assert.Equal(1, destination.Children.Count);
        }

        #region Nested type: Destination

        public class Destination
        {
            public IList<DestinationChild> Children { get; set; }
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