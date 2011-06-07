namespace OoMapper.Tests
{
	using Xunit;

	public class IncludedMappingShouldInheritBaseMappings
	{
		public IncludedMappingShouldInheritBaseMappings()
		{
			Mapper.Reset();
		}

        [Fact]
        public void included_mapping_should_inherit_base_mappings_should_not_throw()
        {
            Mapper.CreateMap<ModelObject, DtoObject>()
                .ForMember(d => d.BaseString, m => m.MapFrom(s => s.DifferentBaseString))
                .Include<ModelSubObject, DtoSubObject>();
            Mapper.CreateMap<ModelSubObject, DtoSubObject>();

            var source = new ModelSubObject
                             {
                                 DifferentBaseString = "123",
                                 SubString = "456"
                             };
            DtoSubObject dto = Mapper.Map<ModelSubObject, DtoSubObject>(source);

            Assert.Equal("123", dto.BaseString);
            Assert.Equal("456", dto.SubString);
        }

		[Fact]
		public void more_specific_map_should_override_base_ignore()
		{
			Mapper.CreateMap<ModelObject, DtoObject>()
				.ForMember(d => d.BaseString, m => m.Ignore())
				.Include<ModelSubObject, DtoSubObject>();

			Mapper.CreateMap<ModelSubObject, DtoSubObject>()
				.ForMember(d => d.BaseString, m => m.MapFrom(s => s.DifferentBaseString));

			var source = new ModelSubObject
							{
								DifferentBaseString = "123",
								SubString = "456"
							};
			DtoSubObject dto = Mapper.Map<ModelSubObject, DtoSubObject>(source);

			Assert.Equal("123", dto.BaseString);
			Assert.Equal("456", dto.SubString);
		}

		[Fact]
		public void more_specific_map_should_override_base_mapping()
		{
			Mapper.CreateMap<ModelObject, DtoObject>()
				.ForMember(d => d.BaseString, m => m.MapFrom(s => s.DifferentBaseString))
				.Include<ModelSubObject, DtoSubObject>();
			Mapper.CreateMap<ModelSubObject, DtoSubObject>()
				.ForMember(d => d.BaseString, m => m.MapFrom(s => "789"));

			DtoSubObject dto = Mapper.Map<ModelSubObject, DtoSubObject>(new ModelSubObject
																			{
																				DifferentBaseString = "123",
																				SubString = "456"
																			});

			Assert.Equal("789", dto.BaseString);
			Assert.Equal("456", dto.SubString);
		}

		[Fact]
		public void included_mapping_should_not_inherit_base_mappings_for_other()
		{
			Mapper.CreateMap<ModelObject, DtoObject>()
				.ForMember(d => d.BaseString, m => m.MapFrom(s => s.DifferentBaseString))
				.Include<ModelSubObject, DtoSubObject>();

			Mapper.CreateMap<ModelSubObject, OtherDto>();

			var modelSubObject = new ModelSubObject
									{
										DifferentBaseString = "123",
										SubString = "456"
									};
			OtherDto dto = Mapper.Map<ModelSubObject, OtherDto>(modelSubObject);

			Assert.Equal("456", dto.SubString);
		}

		[Fact]
		public void include_should_allow_automapper_to_select_more_specific_included_type()
		{
			Mapper.CreateMap<ModelObject, DtoObject>()
				.ForMember(d => d.BaseString, m => m.MapFrom(s => s.DifferentBaseString))
				.Include<ModelSubObject, DtoSubObject>();

			Mapper.CreateMap<ModelSubObject, DtoSubObject>();

			DtoObject dto = Mapper.Map<ModelObject, DtoObject>(new ModelSubObject
																{
																	DifferentBaseString = "123",
																	SubString = "456"
																});

			Assert.IsType<DtoSubObject>(dto);
		}

		#region Nested type: DtoObject

		public class DtoObject
		{
			public string BaseString { get; set; }
		}

		#endregion

		#region Nested type: DtoSubObject

		public class DtoSubObject : DtoObject
		{
			public string SubString { get; set; }
		}

		#endregion

		#region Nested type: ModelObject

		public class ModelObject
		{
			public string DifferentBaseString { get; set; }
		}

		#endregion

		#region Nested type: ModelSubObject

		public class ModelSubObject : ModelObject
		{
			public string SubString { get; set; }
		}

		#endregion

		#region Nested type: OtherDto

		public class OtherDto
		{
			public string SubString { get; set; }
		}

		#endregion
	}
}
