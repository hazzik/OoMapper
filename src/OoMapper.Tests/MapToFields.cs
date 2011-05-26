namespace OoMapper.Tests
{
	using Xunit;

	public class MapToFields
	{
		[Fact]
		public void MapToField()
		{
			Mapper.Reset();
			Mapper.CreateMap<Source, Destination>();

			var source = new Source
			             	{
			             		SomeProperty = 123
			             	};
			Destination destination = Mapper.Map<Source, Destination>(source);
			Assert.Equal(123, destination.SomeProperty);
		}

		#region Nested type: Destination

		private class Destination
		{
			public int SomeProperty;
		}

		#endregion

		#region Nested type: Source

		private class Source
		{
			public int SomeProperty { get; set; }
		}

		#endregion
	}
}