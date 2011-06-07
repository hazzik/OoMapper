namespace OoMapper.Tests
{
	using System.Collections.Generic;
	using Xunit;

	public class ConventionMappedCollectionShouldMapBaseTypes
	{
		public class ItemBase { }

		public class GeneralItem : ItemBase { }

		public class SpecificItem : ItemBase { }

		public class Container
		{
			public Container()
			{
				Items = new List<ItemBase>();
			}
			public List<ItemBase> Items { get; private set; }
		}

		public class ItemDto { }

		public class GeneralItemDto : ItemDto { }

		public class SpecificItemDto : ItemDto { }

		public class ContainerDto
		{
			public ContainerDto()
			{
				Items = new List<ItemDto>();
			}
			public List<ItemDto> Items { get; private set; }
		}

		[Fact]
		public void item_collection_should_map_by_base_type()
		{
			Mapper.Reset();

			Mapper.CreateMap<Container, ContainerDto>();
			
			Mapper.CreateMap<GeneralItem, GeneralItemDto>();
			Mapper.CreateMap<SpecificItem, SpecificItemDto>();

			Mapper.CreateMap<ItemBase, ItemDto>()
				.Include<GeneralItem, GeneralItemDto>()
				.Include<SpecificItem, SpecificItemDto>();

			var dto = Mapper.Map<Container, ContainerDto>(new Container
			                                              	{
			                                              		Items =
			                                              			{
			                                              				new GeneralItem(),
			                                              				new SpecificItem()
			                                              			}
			                                              	});

			Assert.IsType(typeof(GeneralItemDto), dto.Items[0]);
			Assert.IsType(typeof(SpecificItemDto), dto.Items[1]);
		}
	}
}