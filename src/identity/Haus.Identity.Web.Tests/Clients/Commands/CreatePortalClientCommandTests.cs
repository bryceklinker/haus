using Haus.Identity.Web.Clients.Commands;
using Xunit;

namespace Haus.Identity.Web.Tests.Clients.Commands
{
    public class CreatePortalClientCommandTests
    {
        [Fact]
        public void WhenCreatedThenClientCommandHasPortalName()
        {
            var clientCommand = new CreatePortalClientCommand("https://localhost:5001")
                .CreateClientCommand;

            Assert.Equal("Haus Portal", clientCommand.Name);
        }

        [Fact]
        public void WhenCreatedThenClientCommandHasRedirectUri()
        {
            var clientCommand = new CreatePortalClientCommand("https://localhost:5001")
                .CreateClientCommand;
            
            Assert.Contains("https://localhost:5001", clientCommand.RedirectUri);
        }
    }
}