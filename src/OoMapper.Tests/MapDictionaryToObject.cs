using System.Collections.Generic;
using Xunit;

namespace OoMapper.Tests
{
    public class MapDictionaryToObject
    {
        [Fact]
        public void Test()
        {
            Mapper.Reset();
            Mapper.CreateMap<IDictionary<string, object>, Destination>();

            var objects = new Dictionary<string, object>
                              {
                                  {"Property", 100}
                              };

            Destination destination = Mapper.Map<IDictionary<string, object>, Destination>(objects);
            Assert.Equal(100, destination.Property);
        }

        #region Nested type: Destination

        public class Destination
        {
            public int Property { get; set; }
        }

        #endregion
    }
}