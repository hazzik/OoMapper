namespace OoMapper
{
    using System;
    using System.Reflection;

    public class PropertyMapConfiguration : IPropertyMapConfiguration
    {
        private readonly Func<MemberInfo, bool> predicate;
        private bool isIgnored;
        private ISourceMemberResolver sourceMemberResolver;

        public PropertyMapConfiguration(Func<MemberInfo, bool> predicate, int order)
        {
            this.predicate = predicate;
            Order = order;
        }

        public ISourceMemberResolver Resolver
        {
            get { return sourceMemberResolver; }
        }

        public int Order { get; private set; }

        public bool IsApplicableTo(MemberInfo mi)
        {
            return predicate(mi);
        }

        public void Ignore()
        {
            isIgnored = true;
        }

        public bool IsIgnored()
        {
            return isIgnored;
        }

        public bool IsMapped()
        {
            return isIgnored || sourceMemberResolver != null;
        }

        public void SetCustomResolver(ISourceMemberResolver resolver)
        {
            sourceMemberResolver = resolver;
        }
    }
}