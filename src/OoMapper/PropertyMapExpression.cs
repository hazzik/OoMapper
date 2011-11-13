using System;
using System.Linq.Expressions;

namespace OoMapper
{
    public class PropertyMapExpression<TSource>
    {
        private readonly IPropertyMapConfiguration pmc;

        public PropertyMapExpression(IPropertyMapConfiguration pmc)
        {
            this.pmc = pmc;
        }

        public void Ignore()
        {
            pmc.Ignore();
        }

        public void MapFrom<TProperty>(Expression<Func<TSource, TProperty>> sourceMember)
        {
            var tryCatch = Expression.TryCatch(sourceMember.Body,
                                               Expression.Catch(typeof (ArgumentNullException), Expression.Default(sourceMember.Body.Type)),
                                               Expression.Catch(typeof (NullReferenceException), Expression.Default(sourceMember.Body.Type)));

            var lambda = Expression.Lambda(tryCatch, sourceMember.Parameters);

            pmc.SetCustomResolver(new CompositeSourceMemberResolver(new LambdaSourceMemberResolver(lambda), new ConvertSourceMemberResolver()));
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