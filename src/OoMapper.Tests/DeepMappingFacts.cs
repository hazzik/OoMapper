namespace OoMapper.Tests
{
	using Xunit;

	public class DeepMappingFacts
	{
		[Fact]
		public void Test()
		{
			Mapper.Reset();
			Mapper.CreateMap<ComplexSourceChild, ComplexDestinationChild>();
			Mapper.CreateMap<ComplexSource, ComplexDestination>();

			var source = new ComplexSource
			             	{
			             		Some = new ComplexSourceChild {Property = "hello world"}
			             	};

			ComplexDestination map =
				Mapper.Map<ComplexSource, ComplexDestination>(source);

			Assert.Equal("hello world", map.Some.Property);
		}

		private class ComplexSource
		{
			public ComplexSourceChild Some { get; set; }
		}

		private class ComplexSourceChild
		{
			public string Property { get; set; }
		}
		
		private class ComplexDestinationChild
		{
			public string Property { get; set; }
		}

		private class ComplexDestination
		{
			public ComplexDestinationChild Some { get; set; }
		}
	}
}