namespace OoMapper
{
    using System.Reflection;

    public interface IPropertyMapConfiguration
    {
        ISourceMemberResolver Resolver { get; }
        int Order { get; }
        void Ignore();
        bool IsApplicableTo(MemberInfo mi);
        bool IsIgnored();
        bool IsMapped();
        void SetCustomResolver(ISourceMemberResolver resolver);
    }
}