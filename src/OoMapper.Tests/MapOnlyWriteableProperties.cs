using Xunit;

namespace OoMapper.Tests
{
    public class MapOnlyWriteableProperties
    {
        [Fact]
        public void Test()
        {
            Mapper.Reset();
            Mapper.CreateMap<Source, Destination>();
            Mapper.Map<Source, Destination>(new Source
                                                {
                                                    Property = 100,
                                                });
        }

        #region Nested type: Destination

        public class Destination
        {
            public int Property
            {
                get { return 0; }
            }
        }

        #endregion

        #region Nested type: Source

        public class Source
        {
            public int Property { get; set; }
        }

        #endregion
    }
}