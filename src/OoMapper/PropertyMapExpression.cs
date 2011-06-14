using System;
using System.Linq.Expressions;

namespace OoMapper
{
    public class PropertyMapExpression<TSource>
    {
        private readonly PropertyMapConfiguration pmc;

        public PropertyMapExpression(PropertyMapConfiguration pmc)
        {
            this.pmc = pmc;
        }

        public void Ignore()
        {
            pmc.Ignore();
        }

        public void MapFrom<TProperty>(Expression<Func<TSource, TProperty>> sourceMember)
        {
            pmc.SetCustomResolver(new LambdaSourceMemberResolver(sourceMember));
        }

    	public void UseValue(object o)
    	{
    		MapFrom(x => o);
    	}

		public void UseValue<TValue>(TValue value)
    	{
    		MapFrom(x => value);
    	}
    }
}