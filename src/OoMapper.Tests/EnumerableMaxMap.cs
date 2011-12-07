using System.Collections.Generic;
using Xunit;

namespace OoMapper.Tests
{
    public class EnumerableMaxMap : TestBase
    {
        [Fact]
        public void NewFact()
        {
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
            Assert.Equal(100, categoryDto.PricesMax);
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
            public int PricesMax { get; set; }
        }

        #endregion

        #region Nested type: Product

        public class Product
        {
        }

        #endregion
    }
}