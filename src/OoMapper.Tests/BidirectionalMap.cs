namespace OoMapper.Tests
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using Xunit;

	public class BidirectionalMap
	{
		[Fact]
		public void SholudMapParentWithEmptyChildren()
		{
			Mapper.Reset();
			Mapper.CreateMap<Child, ChildDto>();
			Mapper.CreateMap<Parent, ParentDto>();

			Mapper.Map<Parent, ParentDto>(new Parent());
		}

		[Fact]
		public void SholudMapParentWithChildrenWhenChildsParentIsNotSet()
		{
			Mapper.Reset();
			Mapper.CreateMap<Child, ChildDto>();
			Mapper.CreateMap<Parent, ParentDto>();

			Mapper.Map<Parent, ParentDto>(new Parent
			                              	{
			                              		Children = new[]
			                              		           	{
			                              		           		new Child(),
			                              		           	}
			                              	});
		}

		[Fact]
		public void ShouldMapComplexGraphWithRings()
		{
			Mapper.Reset();
			Mapper.CreateMap<Child, ChildDto>();
			Mapper.CreateMap<Parent, ParentDto>();

			var parent = new Parent();
			parent.Children = new[]
			                  	{
			                  		new Child
			                  			{
			                  				Parent = parent
			                  			},
			                  	};
			var parentDto = Mapper.Map<Parent, ParentDto>(parent);
			var dto = parentDto.Children.First().Parent;
			Assert.Equal(parentDto, dto);
		}

		public class Child
		{
			public Parent Parent { get; set; }
		}

		public class ChildDto
		{
			public ParentDto Parent { get; set; }
		}

		public class Parent
		{
			public IEnumerable<Child> Children { get; set; }
		}

		public class ParentDto
		{
			public IEnumerable<ChildDto> Children { get; set; }
		}
	}
}
