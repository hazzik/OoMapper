using Xunit;

namespace OoMapper.Tests
{
    public class MapFromFields : TestBase
	{
		[Fact]
		public void MapFromField()
		{
			Mapper.CreateMap<Source, Destination>();

			var source = new Source
			             	{
			             		SomeField = 123
			             	};
			Destination destination = Mapper.Map<Source, Destination>(source);
			Assert.Equal(123, destination.SomeField);
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
			public int SomeField { get; set; }
			public int Static { get; set; }
		}

		#endregion

		#region Nested type: Source

		public class Source
		{
			public static int Static = 100;
			public int SomeField;
		}

		#endregion
	}
}