namespace OoMapper.Tests.Conventions
{
    using Xunit;

    public class ForMembersTests
    {
        [Fact]
        public void FactMethodName()
        {
            Mapper.CreateMap<Source, Destination>()
                .ForMembers(m => m.Name.EndsWith("Ignored"), opt => opt.Ignore());

            var destination = Mapper.Map<Source, Destination>(new Source());
            
            Assert.Null(destination.ShouldBeIgnored);
        } 
    }

    public class Destination
    {
        public string ShouldBeIgnored { get; set; }
    }

    public class Source
    {
        public string ShouldBeIgnored { get { return "Ignore me!"; } }
    }
}