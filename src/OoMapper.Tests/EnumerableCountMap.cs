using System.Collections.Generic;
using Xunit;

namespace OoMapper.Tests
{
    public class EnumerableCountMap : TestBase
    {
        [Fact]
        public void NewFact()
        {
            Mapper.CreateMap<Category, CategoryDto>();
            CategoryDto categoryDto = Mapper.Map<Category, CategoryDto>(new Category
                                                                            {
                                                                                Products = new[]
                                                                                               {
                                                                                                   new Product(),
                                                                                                   new Product(),
                                                                                               }
                                                                            });
            Assert.Equal(2, categoryDto.ProductsCount);
        }

        #region Nested type: Category

        public class Category
        {
            public IEnumerable<Product> Products { get; set; }
        }

        #endregion

        #region Nested type: CategoryDto

        public class CategoryDto
        {
            public int ProductsCount { get; set; }
        }

        #endregion

        #region Nested type: Product

        public class Product
        {
        }

        #endregion
    }
}