using System.Collections.Generic;
using Xunit;

namespace OoMapper.Tests
{
    public class MapObjectToDictionary
    {
        [Fact]
        public void Test()
        {
            var source = new Source
            {
                Value1 = "Test" ,
                Value2 = 42
            };
            var destination = Mapper.Map<Source, Dictionary<string, object>>(source);
            Assert.Equal(2, destination.Values.Count);
            Assert.True(destination.ContainsKey("Value1"));
            Assert.Equal("Test", destination["Value1"]);
            Assert.True(destination.ContainsKey("Value2"));
            Assert.Equal(42, destination["Value2"]);
        }

        [Fact]
        public void TestToSttring()
        {
            var source = new Source
            {
                Value1 = "Test",
                Value2 = 42
            };
            var destination = Mapper.Map<Source, Dictionary<string, string>>(source);
            Assert.Equal(2, destination.Values.Count);
            Assert.True(destination.ContainsKey("Value1"));
            Assert.Equal("Test", destination["Value1"]);
            Assert.True(destination.ContainsKey("Value2"));
            Assert.Equal("42", destination["Value2"]);
        }

        public class Source
        {
            public string Value1 { get; set; } 
            public int Value2 { get; set; } 
        }
    }
}