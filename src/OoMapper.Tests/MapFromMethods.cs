using Xunit;

namespace OoMapper.Tests
{
	public class MapFromMethods : TestBase
	{
		[Fact]
		public void ShouldMapFromMethodToProperties()
		{
			Mapper.CreateMap<Source, Destination>();

			Destination destination = Mapper.Map<Source, Destination>(new Source());

			Assert.Equal(100, destination.SomeValue);
		}

		[Fact]
		public void ShouldMapFromMethodToFields()
		{
			Mapper.CreateMap<Source, Destination>();

			Destination destination = Mapper.Map<Source, Destination>(new Source());

			Assert.Equal(200, destination.OtherValue);
		}

		[Fact]
		public void ShouldNotMapFromStaticMethods()
		{
			Mapper.CreateMap<Source, Destination>();

			Destination destination = Mapper.Map<Source, Destination>(new Source());
			Assert.Equal(0, destination.StaticMethod);
		}

		[Fact]
		public void ShouldNotMapFromVoidMethods()
		{
			Mapper.CreateMap<Source, Destination>();

			var source = new Source();
			Destination destination = Mapper.Map<Source, Destination>(source);
			Assert.False(source.VoidMethodCalled);
			Assert.Null(destination.VoidMethod);
		}

		#region Nested type: Destination

		public class Destination
		{
			public int OtherValue;
			public int SomeValue { get; set; }
			public int StaticMethod { get; set; }
			public object VoidMethod { get; set; }
		}

		#endregion

		#region Nested type: Source

		public class Source
		{
			public bool VoidMethodCalled { get; private set; }

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

			public static int StaticMethod()
			{
				return 300;
			}

			public virtual void VoidMethod()
			{
				VoidMethodCalled = true;
			}
		}

		#endregion
	}
}