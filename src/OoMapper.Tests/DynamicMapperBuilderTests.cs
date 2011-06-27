using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Moq;
using Xunit;

namespace OoMapper.Tests
{
	public class DynamicMapperBuilderTests
    {
		[Fact(Skip = "wont fix")]
        public void DynamicMapper()
        {
    	    var mockMapperConfiguration = new Mock<IMappingConfiguration>();
    		mockMapperConfiguration.Setup(z => z.BuildExisting(typeof (string), typeof (string))).Returns((Expression<Func<string, string, string>>) ((y, x) => string.Format("{0}{1}", y, 1))).Verifiable();
    		mockMapperConfiguration.Setup(z => z.BuildExisting(typeof (int), typeof (int))).Returns((Expression<Func<int, int, int>>) ((y, x) => y + 1)).Verifiable();
    	    
			var type = DynamicMapperBuilder.Create().CreateDynamicMapper(new[]
    	                                                                     {
    	                                                                         new TypeMapConfiguration(typeof (int), typeof (int)),
    	                                                                         new TypeMapConfiguration(typeof (string), typeof (string)),
    	                                                                     });

    	    var instance = (DynamicMapperBase) Activator.CreateInstance(type, mockMapperConfiguration.Object);
    		var map1 = (int) instance.DynamicMap(1);
    		var map2 = (string) instance.DynamicMap("hello");

            Assert.Equal(2, map1);
    		Assert.Equal("hello1", map2);

            mockMapperConfiguration.Verify(z => z.BuildExisting(typeof (object), typeof (object)), Times.Once());
        }

		[Fact(Skip = "wont fix")]
		public void QueryableWithDynamicMapper()
        {
            var mockMapperConfiguration = new Mock<IMappingConfiguration>();
			mockMapperConfiguration.Setup(z => z.BuildExisting(typeof(string), typeof(string))).Returns((Expression<Func<string, string, string>>)((y, x) => string.Format("{0}{1}", y, 1))).Verifiable();
			mockMapperConfiguration.Setup(z => z.BuildExisting(typeof(int), typeof(int))).Returns((Expression<Func<int, int, int>>)((y, x) => y + 1)).Verifiable();

        	var type = DynamicMapperBuilder.Create().CreateDynamicMapper(new[]
        	                                                             	{
    	                                                                         new TypeMapConfiguration(typeof (int), typeof (int)),
    	                                                                         new TypeMapConfiguration(typeof (string), typeof (string)),
        	                                                             	});

            var instance = (DynamicMapperBase)Activator.CreateInstance(type, mockMapperConfiguration.Object);

            var query = new object[]
                            {
                                1, "hello"
                            };

            ParameterExpression src = Expression.Parameter(query.GetType(), "src");

        	Expression<Func<object, object>> expression = x => instance.DynamicMap(x);

            var lambdaExpression = Expression.Lambda<Func<object[], IEnumerable<object>>>(Expression.Call(typeof(Enumerable), "Select", new[] { typeof(object), typeof(object) }, src, expression), src);
            var compile = lambdaExpression.Compile();
            object[] map = compile.Invoke(query).ToArray();

            Assert.Equal(2, map.First());
            Assert.Equal("hello1", map.Last());

			mockMapperConfiguration.Verify(z => z.BuildExisting(typeof(object), typeof(object)), Times.Once());
        }
    }
}