using Xunit;

namespace OoMapper.Tests
{
	public class MapFromProperties : TestBase
	{
		[Fact]
		public void ShouldNotMapFromWriteOnlyFields()
		{
			Mapper.CreateMap<Source, Destination>();

			var source = new Source
			             	{
			             		SomeValue = 123
			             	};
			Destination destination = Mapper.Map<Source, Destination>(source);
			Assert.NotEqual(123, destination.SomeValue);
		}

		#region Nested type: Destination

		public class Destination
		{
			public int SomeValue { get; set; }
		}

		#endregion

		#region Nested type: Source

		public class Source
		{
			public int SomeValue
			{
				set { }
			}
		}

		#endregion
	}
}