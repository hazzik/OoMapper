namespace OoMapper.Tests
{
	using Xunit;

    public class DeepInheritance : TestBase
	{
		[Fact]
		public void FactName()
		{
			Mapper.CreateMap<Source, SourceDto>()
				.ForMember(x => x.Value1, opt => opt.UseValue(1))
				.Include<Child, ChildDto>();

			Mapper.CreateMap<Child, ChildDto>()
				.ForMember(x => x.Value2, opt => opt.UseValue(2))
				.Include<GrandChild, GrandChildDto>();

			Mapper.CreateMap<GrandChild, GrandChildDto>()
				.ForMember(x => x.Value3, opt => opt.UseValue(3));

			var sourceDto = Mapper.Map<GrandChild, GrandChildDto>(new GrandChild());
			Assert.Equal(1, sourceDto.Value1);
			Assert.Equal(2, sourceDto.Value2);
			Assert.Equal(3, sourceDto.Value3);
		}

		[Fact]
		public void FactName5()
		{
			Mapper.CreateMap<Source, SourceDto>()
				.ForMember(x => x.Value1, opt => opt.UseValue(1))
				.Include<Child, ChildDto>()
				.Include<GrandChild, GrandChildDto>();

			Mapper.CreateMap<Child, ChildDto>()
				.ForMember(x => x.Value1, opt => opt.UseValue(0))
				.ForMember(x => x.Value2, opt => opt.UseValue(2))
				.Include<GrandChild, GrandChildDto>();

			Mapper.CreateMap<GrandChild, GrandChildDto>()
				.ForMember(x => x.Value3, opt => opt.UseValue(3));

			var sourceDto = (GrandChildDto)Mapper.Map<Source, SourceDto>(new GrandChild());
			Assert.Equal(0, sourceDto.Value1);
			Assert.Equal(2, sourceDto.Value2);
			Assert.Equal(3, sourceDto.Value3);
		}
		
		[Fact]
		public void FactName2()
		{
			Mapper.CreateMap<Source, SourceDto>()
				.ForMember(x => x.Value1, opt => opt.UseValue(1))
				.Include<Child, ChildDto>();

			Mapper.CreateMap<Child, ChildDto>()
				.ForMember(x => x.Value2, opt => opt.UseValue(2))
				.Include<GrandChild, GrandChildDto>();

			Mapper.CreateMap<GrandChild, GrandChildDto>()
				.ForMember(x => x.Value3, opt => opt.UseValue(3));

			var sourceDto = (GrandChildDto)Mapper.Map<Source, SourceDto>(new GrandChild());
			Assert.Equal(1, sourceDto.Value1);
			Assert.Equal(2, sourceDto.Value2);
			Assert.Equal(3, sourceDto.Value3);
		}
		
		[Fact]
		public void FactName3()
		{
			Mapper.CreateMap<GrandChild, GrandChildDto>()
				.ForMember(x => x.Value3, opt => opt.UseValue(3));

			Mapper.CreateMap<Child, ChildDto>()
				.ForMember(x => x.Value2, opt => opt.UseValue(2))
				.Include<GrandChild, GrandChildDto>();

			Mapper.CreateMap<Source, SourceDto>()
				.ForMember(x => x.Value1, opt => opt.UseValue(1))
				.Include<Child, ChildDto>();

			var sourceDto = Mapper.Map<GrandChild, GrandChildDto>(new GrandChild());
			Assert.Equal(1, sourceDto.Value1);
			Assert.Equal(2, sourceDto.Value2);
			Assert.Equal(3, sourceDto.Value3);
		}
		
		[Fact]
		public void FactName4()
		{
			Mapper.CreateMap<GrandChild, GrandChildDto>()
				.ForMember(x => x.Value3, opt => opt.UseValue(3));

			Mapper.CreateMap<Child, ChildDto>()
				.ForMember(x => x.Value2, opt => opt.UseValue(2))
				.Include<GrandChild, GrandChildDto>();

			Mapper.CreateMap<Source, SourceDto>()
				.ForMember(x => x.Value1, opt => opt.UseValue(1))
				.Include<Child, ChildDto>();

			var sourceDto = (GrandChildDto)Mapper.Map<Source, SourceDto>(new GrandChild());
			Assert.Equal(1, sourceDto.Value1);
			Assert.Equal(2, sourceDto.Value2);
			Assert.Equal(3, sourceDto.Value3);
		}

		[Fact]
		public void FactName6()
		{
			Mapper.CreateMap<Source, SourceDto>()
				.ForMember(x => x.Value1, opt => opt.UseValue(1))
				.Include<Child, ChildDto>();

			Mapper.CreateMap<Child, ChildDto>()
				.ForAllMembers(x => x.Ignore())
				.Include<GrandChild, GrandChildDto>();

			Mapper.CreateMap<GrandChild, GrandChildDto>()
				.ForMember(x => x.Value3, opt => opt.UseValue(3));

			var sourceDto = (GrandChildDto)Mapper.Map<Source, SourceDto>(new GrandChild());
			Assert.Equal(0, sourceDto.Value1);
			Assert.Equal(0, sourceDto.Value2);
			Assert.Equal(3, sourceDto.Value3);
		}

		public class Source { }

		public class Child : Source { }

		public class GrandChild : Child { }

		public class SourceDto
		{
			public int Value1 { get; set; }
		}

		public class ChildDto : SourceDto
		{
			public int Value2 { get; set; }
		}

		public class GrandChildDto : ChildDto
		{
			public int Value3 { get; set; }
		}
	}
}