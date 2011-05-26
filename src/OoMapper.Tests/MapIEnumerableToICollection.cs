using System.Collections.Generic;
using Xunit;

namespace OoMapper.Tests
{
	public class MapIEnumerableToICollection
	{
		[Fact]
		public void Test()
		{
			Mapper.Reset();
			Mapper.CreateMap<SourceChild, DestinationChild>();
			Mapper.CreateMap<Source, Destination>();

			var source = new Source
			             	{
			             		Children = new[]
			             		           	{
			             		           		new SourceChild
			             		           			{
			             		           				A = "hello world"
			             		           			},
			             		           	}
			             	};

			Destination destination = Mapper.Map<Source, Destination>(source);
			Assert.Equal(1, destination.Children.Count);
		}

		#region Nested type: Destination

		private class Destination
		{
			public ICollection<DestinationChild> Children { get; set; }
		}

		#endregion

		#region Nested type: DestinationChild

		private class DestinationChild
		{
			public string A { get; set; }
		}

		#endregion

		#region Nested type: Source

		private class Source
		{
			public IEnumerable<SourceChild> Children { get; set; }
		}

		#endregion

		#region Nested type: SourceChild

		private class SourceChild
		{
			public string A { get; set; }
		}

		#endregion
	}
}