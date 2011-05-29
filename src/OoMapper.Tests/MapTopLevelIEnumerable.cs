using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace OoMapper.Tests
{
    public class MapTopLevelIEnumerable
    {
        [Fact]
        public void Test()
        {
            Mapper.CreateMap<Source, Destination>();
            var sources = new[]
                              {
                                  new Source
                                      {
                                          Value = 100
                                      }
                              };
            var destinations = Mapper.Map<IEnumerable<Source>, IEnumerable<Destination>>(sources);
            Assert.Equal(1, destinations.Count());
        }

        #region Nested type: Destination

        private class Destination
        {
            public int Value { get; set; }
        }

        #endregion

        #region Nested type: Source

        private class Source
        {
            public int Value { get; set; }
        }

        #endregion
    }
}