using Haus.Identity.Core.Common.Messaging;
using Haus.Identity.Core.Common.Messaging.Commands;

namespace Haus.Identity.Core.IdentityResources.CreateIdentityResource
{
    public class CreateIdentityResourceCommand : ICommand<CreateIdentityResourceResult>
    {
        public string Name { get; }

        public string DisplayName { get; }

        public string[] ClaimTypes { get; }

        public CreateIdentityResourceCommand(string name, string displayName, string[] claimTypes)
        {
            Name = name;
            DisplayName = displayName;
            ClaimTypes = claimTypes;
        }
    }
}