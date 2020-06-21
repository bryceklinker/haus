using Haus.Identity.Core.Common.Messaging.Commands;
using IdentityServer4.Models;

namespace Haus.Identity.Core.IdentityResources.CreateIdentityResource
{
    public class CreateDefaultIdentityResourceCommand : ICommand
    {
        public IdentityResource[] Resources { get; }

        public CreateDefaultIdentityResourceCommand(params IdentityResource[] resources)
        {
            Resources = resources;
        }
    }
}