using Haus.Cqrs.Commands;

namespace Haus.Identity.Web.Clients.Commands
{
    public class CreatePortalClientCommand : ICommand
    {
        public CreateClientCommand CreateClientCommand { get; }
        
        public CreatePortalClientCommand(string redirectUri)
        {
            CreateClientCommand = new CreateClientCommand("Haus Portal", redirectUri: redirectUri);
        }
    }
}