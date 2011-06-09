namespace OoMapper.Tests
{
	using Xunit;

	public class MapFromMethods
	{
		[Fact]
		public void FactName()
		{
			Mapper.Reset();
			Mapper.CreateMap<Source, Destination>();
			Destination destination = Mapper.Map<Source, Destination>(new Source());
			Assert.Equal(100, destination.SomeValue);
			Assert.Equal(200, destination.OtherValue);
		}

		#region Nested type: Destination

		public class Destination
		{
			public int OtherValue;
			public int SomeValue { get; set; }
		}

		#endregion

		#region Nested type: Source

		public class Source
		{
			public int SomeValue()
			{
				return 100;
			}

			public int SomeValue(int x)
			{
				return 500;
			}

			public int OtherValue()
			{
				return 200;
			}
		}

		#endregion
	}
}
