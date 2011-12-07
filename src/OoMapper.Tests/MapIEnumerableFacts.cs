using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace OoMapper.Tests
{
    public class MapIEnumerableFacts   :TestBase
    {
        [Fact]
        public void ShouldBeAbleToMap()
        {
            Mapper.CreateMap<SourceChild, DestinationChild>();
            Mapper.CreateMap<Source, Destination>()
                .ForMember(x => x.A, opt => opt.MapFrom(x => X(x)));

            var source = new Source
                             {
                                 B = new List<SourceChild> {new SourceChild(), new SourceChild()}
                             };

            Destination destination = Mapper.Map<Source, Destination>(source);
            Assert.Equal(2, destination.A.Count());
        }

        [Fact]
        public void ShouldBeAbleToMapNullEnumerable()
        {
            Mapper.CreateMap<SourceChild, DestinationChild>();
            Mapper.CreateMap<Source, Destination>()
                .ForMember(x => x.A, opt => opt.MapFrom(x => X(x)));

            var source = new Source();

            Destination destination = Mapper.Map<Source, Destination>(source);
            Assert.Null(destination.A);
        }

        private static IEnumerable<SourceChild> X(Source x)
        {
            return x.B.Where(i => true).Select(i => i);
        }

        #region Nested type: Destination

        public class Destination
        {
            public IEnumerable<DestinationChild> A { get; set; }
        }

        #endregion

        #region Nested type: DestinationChild

        public class DestinationChild
        {
        }

        #endregion

        #region Nested type: Source

        public class Source
        {
            public IEnumerable<SourceChild> B { get; set; }
        }

        #endregion

        #region Nested type: SourceChild

        public class SourceChild
        {
        }

        #endregion
    }
}