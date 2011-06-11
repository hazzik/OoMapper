using System.Collections.Generic;
using Xunit;

namespace OoMapper.Tests
{
    public class EnumerableAnyMap
    {
        [Fact]
        public void True()
        {
            Mapper.Reset();
            Mapper.CreateMap<Category, CategoryDto>();
            CategoryDto categoryDto = Mapper.Map<Category, CategoryDto>(new Category
                                                                            {
                                                                                Prices = new[]
                                                                                             {
                                                                                                 1,
                                                                                                 100,
                                                                                                 2
                                                                                             }
                                                                            });
            Assert.True(categoryDto.PricesAny);
        }
        [Fact]
        public void False()
        {
            Mapper.Reset();
            Mapper.CreateMap<Category, CategoryDto>();
            CategoryDto categoryDto = Mapper.Map<Category, CategoryDto>(new Category
                                                                            {
                                                                                Prices = new int[]
                                                                                             {
                                                                                                 
                                                                                             }
                                                                            });
            Assert.False(categoryDto.PricesAny);
        }

        #region Nested type: Category

        public class Category
        {
            public IEnumerable<int> Prices { get; set; }
        }

        #endregion

        #region Nested type: CategoryDto

        public class CategoryDto
        {
            public bool PricesAny { get; set; }
        }

        #endregion

        #region Nested type: Product

        public class Product
        {
        }

        #endregion
    }
}