using Xunit;

namespace OoMapper.Tests
{
	public class MapFromProperties : TestBase
	{
		[Fact]
		public void ShouldNotMapFromWriteOnlyProperties()
		{
			Mapper.CreateMap<Source, Destination>();

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
			Mapper.CreateMap<Source, Destination>();
			var source = new Source();
			Destination destination = Mapper.Map<Source, Destination>(source);
			Assert.NotEqual(100, destination.Static);
		}

		#region Nested type: Destination

		public class Destination
		{
			public int SomeValue { get; set; }
			public int Static { get; set; }
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
		}

		#endregion
	}
}