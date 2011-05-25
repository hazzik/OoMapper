using System.Linq.Expressions;
using System.Reflection;

namespace OoMapper
{
    public class PropertyMap
    {
        private readonly PropertyInfo destinationProperty;
        public ISourceMemberResolver SourceMemberResolver { get; set; }

        public PropertyMap(PropertyInfo destinationProperty, ISourceMemberResolver sourceMemberResolver)
        {
            this.destinationProperty = destinationProperty;
            this.SourceMemberResolver = sourceMemberResolver;
        }

        public bool IsIgnored { get; set; }

        public MemberInfo DestinationMember
        {
            get { return destinationProperty; }
        }

        public Expression BuildAssign(Expression destination, Expression source)
        {
            PropertyInfo info = destinationProperty;
            return Expression.Assign(Expression.MakeMemberAccess(destination, info), SourceMemberResolver.BuildSource(source, info.PropertyType));
        }

        public MemberAssignment BuildBind(Expression source)
        {
            PropertyInfo info = destinationProperty;
            return Expression.Bind(info, SourceMemberResolver.BuildSource(source, info.PropertyType));
        }
    }
}