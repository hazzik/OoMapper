using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace OoMapper.Tests
{
    public class MapDictionaryToDictionary
    {
        [Fact]
        public void Test()
        {
            Mapper.CreateMap<SourceChild, DestinationChild>();
            Mapper.CreateMap<Source, Destination>();

            var source = new Source
                             {
                                 Values = new Dictionary<int, SourceChild>
                                              {
                                                  {1, new SourceChild {A = 2}}
                                              }
                             };
            Destination destination = Mapper.Map<Source, Destination>(source);
            Assert.Equal(1, destination.Values.Count);
            Assert.Equal(1, destination.Values.Keys.First());
            Assert.Equal(2, destination.Values.Values.First().A);
        }

        public class Destination
        {
            public IDictionary<int, DestinationChild> Values { get; set; }
        }

        public class DestinationChild
        {
            public int A { get; set; }
        }

        public class Source
        {
            public IDictionary<int, SourceChild> Values { get; set; }
        }

        public class SourceChild
        {
            public int A { get; set; }
        }
    }
}