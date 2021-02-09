using System.Threading.Tasks;
using Haus.Web.Host.Tests.Support;
using Xunit;

namespace Haus.Web.Host.Tests.ServiceLocation
{
    [Collection(HausWebHostCollectionFixture.Name)]
    public class ServiceLocationApiTests
    {
        public ServiceLocationApiTests(HausWebHostApplicationFactory factory)
        {
            
        }

        [Fact]
        public async Task WhenServiceLocationsAreRequestedThenWebServiceLocationIsPublished()
        {
            
        }
    }
}