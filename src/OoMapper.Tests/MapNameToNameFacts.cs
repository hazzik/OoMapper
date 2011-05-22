using System;
using Xunit;

namespace OoMapper.Tests
{
    public class MapNameToNameFacts
    {
        [Fact]
        public void Test()
        {
            Mapper.Configure<Source, Destination>();

            Destination map = Mapper.Map<Source, Destination>(new Source {SomeProperty = "hello world"});

            Assert.Equal("hello world", map.SomeProperty);
        }

        [Fact]
        public void Test2()
        {
            Mapper.Configure<ComplexSource, Destination>();

            Destination map = Mapper.Map<ComplexSource, Destination>(new ComplexSource {Some = new ComplexSource.ComplexSourceChild {Property = "hello world"}});

            Assert.Equal("hello world", map.SomeProperty);
        }

        [Fact]
        public void Test3()
        {
            Mapper.Configure<ComplexSource2, Destination>();

            Destination map = Mapper.Map<ComplexSource2, Destination>(new ComplexSource2 {Some = new ComplexSourceChild2 {Pro = new ComplexSourceChild3 {Perty = "hello world"}}});

            Assert.Equal("hello world", map.SomeProperty);
        }

        #region Nested type: Destination

        internal class Destination
        {
            public string SomeProperty { get; set; }
        }

        #endregion
    }
}
