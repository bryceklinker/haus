using Haus.Cqrs.Commands;
using IdentityServer4.Models;

namespace Haus.Identity.Core.ApiResources.CreateApiResource
{
    public class CreateIdentityApiResourceCommand : ICommand
    {
        public IdentityResource[] Resources { get; }

        public CreateIdentityApiResourceCommand(params IdentityResource[] resources)
        {
            Resources = resources;
        }
    }
}