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

		public class ComplexSource
		{
			public ComplexSourceChild Some { get; set; }
		}

		public class ComplexSourceChild
		{
			public string Property { get; set; }
		}
		
		public class ComplexDestinationChild
		{
			public string Property { get; set; }
		}

		public class ComplexDestination
		{
			public ComplexDestinationChild Some { get; set; }
		}
	}
}