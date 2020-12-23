using System.Threading.Tasks;
using Haus.Testing.Support;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace Haus.Device.Simulator.Test.Lights
{
    public class LightsApiTests : WebApplicationFactory<Startup>
    {
        private readonly WebApplicationFactory<Startup> _factory;

        public LightsApiTests(WebApplicationFactory<Startup> factory)
        {
            _factory = factory;
        }

        [Fact]
        public async Task WhenLightAddedThenLightAnnouncesItsBeenDiscovered()
        {
            var client = _factory.CreateClient();
            
            Eventually.Assert(() =>
            {
            });
        }
    }
}