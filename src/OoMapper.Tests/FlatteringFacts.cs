using Xunit;

namespace OoMapper.Tests
{
    public class FlatteringFacts
    {
        [Fact]
        public void MapMembersOnSameLevelWithEqualNames()
        {
            Mapper.CreateMap<Source, Destination>();

            var source = new Source
                             {
                                 SomeProperty = "hello world"
                             };

            Destination map = Mapper.Map<Source, Destination>(source);

            Assert.Equal("hello world", map.SomeProperty);
        }

        [Fact]
        public void MapMemberWithPartNamesOnDifferentLevels()
        {
            Mapper.CreateMap<ComplexSource, Destination>();

            var source = new ComplexSource
                                    {
                                        Some = new ComplexSourceChild {Property = "hello world"}
                                    };

            Destination map =
                Mapper.Map<ComplexSource, Destination>(source);

            Assert.Equal("hello world", map.SomeProperty);
        }

        [Fact]
        public void MapMemberWithPartNamesOnDeepDifferentLevels()
        {
            Mapper.Reset();
            Mapper.CreateMap<ComplexSource2, Destination>();

            var source = new ComplexSource2
                             {
                                 Some = new ComplexSourceChild2
                                            {
                                                Pro = new ComplexSourceChild3 {Perty = "hello world"}
                                            }
                             };
            Destination map = Mapper.Map<ComplexSource2, Destination>(source);

            Assert.Equal("hello world", map.SomeProperty);
        }

        [Fact]
        public void MapExistingObject()
        {
            Mapper.Reset();
            Mapper.CreateMap<ComplexSource2, Destination>();

            var source = new ComplexSource2
                                     {
                                         Some = new ComplexSourceChild2
                                                    {
                                                        Pro = new ComplexSourceChild3 {Perty = "hello world"}
                                                    }
                                     };
            Destination map = Mapper.Map(source, new Destination());

            Assert.Equal("hello world", map.SomeProperty);
        }


        #region Nested type: ComplexSource

        private class ComplexSource
        {
            public ComplexSourceChild Some { get; set; }
        }

        #endregion

        #region Nested type: ComplexSource2

        private class ComplexSource2
        {
            public ComplexSourceChild2 Some { get; set; }
        }

        #endregion

        #region Nested type: ComplexSourceChild

        private class ComplexSourceChild
        {
            public string Property { get; set; }
        }

        #endregion

        #region Nested type: ComplexSourceChild2

        private class ComplexSourceChild2
        {
            public ComplexSourceChild3 Pro { get; set; }
        }

        #endregion

        #region Nested type: ComplexSourceChild3

        private class ComplexSourceChild3
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