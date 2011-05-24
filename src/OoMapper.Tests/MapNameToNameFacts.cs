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

        #region Nested type: ComplexSource

        internal class ComplexSource
        {
            public ComplexSourceChild Some { get; set; }

            #region Nested type: ComplexSourceChild

            internal class ComplexSourceChild
            {
                public string Property { get; set; }
            }

            #endregion
        }

        #endregion

        #region Nested type: ComplexSource2

        private class ComplexSource2
        {
            public ComplexSourceChild2 Some { get; set; }
        }

        #endregion

        #region Nested type: ComplexSourceChild2

        private class ComplexSourceChild2
        {
            public ComplexSourceChild3 Pro { get; set; }
        }

        #endregion

        #region Nested type: ComplexSourceChild3

        public class ComplexSourceChild3
        {
            public string Perty { get; set; }
        }

        #endregion

        #region Nested type: Destination

        private class Destination
        {
            public string SomeProperty { get; set; }
        }

        #endregion

        #region Nested type: Source

        private class Source
        {
            public string SomeProperty { get; set; }
        }

        #endregion
    }
}
