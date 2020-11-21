using Xunit;

namespace Haus.Web.Host.Tests.Support
{
    [CollectionDefinition(Name)]
    public class HausWebHostCollectionFixture : ICollectionFixture<HausWebHostApplicationFactory>
    {
        public const string Name = "Haus Web Host";
    }
}