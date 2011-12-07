namespace OoMapper
{
    public interface IPropertyMapConfiguration
    {
        string DestinationMemberName { get; }
        ISourceMemberResolver Resolver { get; }
        void Ignore();
        bool IsIgnored();
        bool IsMapped();
        void SetCustomResolver(ISourceMemberResolver resolver);
    }

    public class PropertyMapConfiguration : IPropertyMapConfiguration
    {
        private readonly string destinationMemberName;
        private bool isIgnored;
        private ISourceMemberResolver sourceMemberResolver;

        public PropertyMapConfiguration(string destinationMemberName)
        {
            this.destinationMemberName = destinationMemberName;
        }

        public string DestinationMemberName
        {
            get { return destinationMemberName; }
        }

        public ISourceMemberResolver Resolver
        {
            get { return sourceMemberResolver; }
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