using Xunit;

namespace OoMapper.Tests
{
    public class ForAllMembersFacts
    {
        [Fact]
        public void TestIgnore()
        {
            Mapper.Reset();
            Mapper.CreateMap<Source, Destination>()
                .ForAllMembers(opt => opt.Ignore());

            var source = new Source
                             {
                                 Value1 = "hello",
                                 Value2 = "world"
                             };
            Destination destination = Mapper.Map<Source, Destination>(source);
            Assert.Null(destination.Value1);
            Assert.Null(destination.Value2);
        }
        
        [Fact]
        public void TestSource()
        {
            Mapper.Reset();
            Mapper.CreateMap<Source, Destination>()
                .ForAllMembers(opt => opt.MapFrom(x => "x"));

            var source = new Source
                             {
                                 Value1 = "hello",
                                 Value2 = "world"
                             };
            Destination destination = Mapper.Map<Source, Destination>(source);
            Assert.Equal("x", destination.Value1);
            Assert.Equal("x", destination.Value2);
        }

        #region Nested type: Destination

        public class Destination
        {
            public string Value2;
            public string Value1 { get; set; }
        }

        #endregion

        #region Nested type: Source

        public class Source
        {
            public string Value2;
            public string Value1 { get; set; }
        }

        #endregion
    }
}