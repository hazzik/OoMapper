using System;
using System.Collections.Generic;
using Xunit;

namespace OoMapper.Tests
{
    public class TypeUtilsFacts
    {
        [Fact]
        public void GetElementTypeOfEnumerableOfIDictionaryReturnsKeyValuePair()
        {
            Type elementType = TypeUtils.GetElementTypeOfEnumerable(typeof (Dictionary<int, int>));
            Assert.Equal(typeof (KeyValuePair<int, int>), elementType);
        }
        
		[Fact]
        public void IsDictionaryReturnsTrueForIDictionary()
        {
            bool b = typeof (IDictionary<int, int>).IsDictionary();
			Assert.True(b);
        }

		[Fact]
        public void IsDictionaryReturnsTrueForDictionary()
        {
            bool b = typeof (Dictionary<int, int>).IsDictionary();
			Assert.True(b);
        }
    }
}