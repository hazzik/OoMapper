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
    	[Fact]
        public void DynamicMapper()
        {
            var type = DynamicMapperBuilder.Create().CreateDynamicMapper(new[]
                                                                         	{
                                                                         		new TypeMap(typeof (object), typeof (object), null),
                                                                         		new TypeMap(typeof (string), typeof (string), null),
                                                                         	});

            var mockMapperConfiguration = new Mock<IMappingConfiguration>();
            mockMapperConfiguration.Setup(z => z.BuildNew(typeof (string), typeof (string))).Returns((Expression<Func<string, string>>) (y => string.Format("{0}{1}", y, 1))).Verifiable();
            mockMapperConfiguration.Setup(z => z.BuildNew(typeof (object), typeof (object))).Returns((Expression<Func<object, object>>) (y => ((int) y) + 1)).Verifiable();

    		var instance = (DynamicMapperBase) Activator.CreateInstance(type, mockMapperConfiguration.Object);
    		var map1 = (int) instance.DynamicMap(1);
    		var map2 = (string) instance.DynamicMap("hello");

            Assert.Equal(map1, 2);
            Assert.Equal(map2, "hello1");

            mockMapperConfiguration.Verify(z => z.BuildNew(typeof (object), typeof (object)), Times.Once());
        }

        [Fact]
        public void QueryableWithDynamicMapper()
        {
        	var type = DynamicMapperBuilder.Create().CreateDynamicMapper(new[]
        	                                                             	{
        	                                                             		new TypeMap(typeof (object), typeof (object), null),
        	                                                             		new TypeMap(typeof (string), typeof (string), null),
        	                                                             	});

            var mockMapperConfiguration = new Mock<IMappingConfiguration>();
            mockMapperConfiguration.Setup(z => z.BuildNew(typeof (string), typeof (string))).Returns((Expression<Func<string, string>>) (y => string.Format("{0}{1}", y, 1))).Verifiable();
            mockMapperConfiguration.Setup(z => z.BuildNew(typeof(object), typeof(object))).Returns((Expression<Func<object, object>>)(y => ((int)y) + 1)).Verifiable();

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

            mockMapperConfiguration.Verify(z => z.BuildNew(typeof (object), typeof (object)), Times.Once());
        }
    }
}