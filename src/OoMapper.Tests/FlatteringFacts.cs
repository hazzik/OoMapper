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
        
        [Fact]
        public void NullBehave()
        {
            Mapper.Reset();
            Mapper.CreateMap<ComplexSource2, Destination>();

            var source = new ComplexSource2();

            Destination map = Mapper.Map<ComplexSource2, Destination>(source);

            Assert.Null(map.SomeProperty);
        }
        
        [Fact]
        public void NullBehave2()
        {
            Mapper.Reset();
            Mapper.CreateMap<ComplexSource2, Destination>();

            Destination map = Mapper.Map<ComplexSource2, Destination>(null);

            Assert.Null(map);
        }

        #region Nested type: ComplexSource

        public class ComplexSource
        {
            public ComplexSourceChild Some { get; set; }
        }

        #endregion

        #region Nested type: ComplexSource2

        public class ComplexSource2
        {
            public ComplexSourceChild2 Some { get; set; }
        }

        #endregion

        #region Nested type: ComplexSourceChild

        public class ComplexSourceChild
        {
            public string Property { get; set; }
        }

        #endregion

        #region Nested type: ComplexSourceChild2

        public class ComplexSourceChild2
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

        public class Destination
        {
            public string SomeProperty { get; set; }
        }

        #endregion

        #region Nested type: Source

        public class Source
        {
            public string SomeProperty { get; set; }
        }

        #endregion
    }
}