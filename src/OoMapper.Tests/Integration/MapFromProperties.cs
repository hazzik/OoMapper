using Xunit;

namespace OoMapper.Tests.Integration
{
	public class MapFromProperties : TestBase
	{
		[Fact]
		public void ShouldNotMapFromWriteOnlyProperties()
		{
		    Mapper.CreateMap<Source, Destination>()
		        .ForMember(x => x.Item, opt => opt.Ignore())
		        .ForMember(x => x.SomeValue, opt => opt.Ignore());

		    var source = new Source
			             	{
			             		SomeValue = 123
			             	};
			Destination destination = Mapper.Map<Source, Destination>(source);
			Assert.NotEqual(123, destination.SomeValue);
		}

		[Fact]
		public void ShouldNotMapFromStaticProperties()
		{
		    Mapper.CreateMap<Source, Destination>()
		        .ForMember(x => x.Item, opt => opt.Ignore())
		        .ForMember(x => x.SomeValue, opt => opt.Ignore());
			var source = new Source();
			Destination destination = Mapper.Map<Source, Destination>(source);
			Assert.NotEqual(100, destination.Static);
		}

	    [Fact]
	    public void ShouldNotMapFromIndexer()
	    {
	        Mapper.CreateMap<Source, Destination>()
	            .ForMember(x => x.Static, opt => opt.Ignore())
	            .ForMember(x => x.SomeValue, opt => opt.Ignore());

            var source = new Source();
            Destination destination = Mapper.Map<Source, Destination>(source);
            Assert.NotEqual(250, destination.Item);
        }

		#region Nested type: Destination

		public class Destination
		{
			public int SomeValue { get; set; }
			public int Static { get; set; }
            public int Item { get; set; }
		}

		#endregion

		#region Nested type: Source

		public class Source
		{
			public int SomeValue
			{
				set { }
			}

			public static int Static
			{
				get { return 100; }
			}

		    public int this[int index]
		    {
                get { return 250; }
		    }
        }

		#endregion
	}
}