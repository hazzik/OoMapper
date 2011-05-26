namespace OoMapper.Tests
{
	using Xunit;

	public class MapFromFields
	{
		[Fact]
		public void MapFromField()
		{
			Mapper.Reset();
			Mapper.CreateMap<Source, Destination>();

			var source = new Source
			             	{
			             		SomeField = 123
			             	};
			Destination destination = Mapper.Map<Source, Destination>(source);
			Assert.Equal(123, destination.SomeField);
		}

		#region Nested type: Destination

		private class Destination
		{
			public int SomeField { get; set; }
		}

		#endregion

		#region Nested type: Source

		private class Source
		{
			public int SomeField;
		}

		#endregion
	}
}