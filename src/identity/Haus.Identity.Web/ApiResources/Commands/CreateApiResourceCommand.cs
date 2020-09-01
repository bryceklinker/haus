using System.Linq;
using Haus.Cqrs.Commands;
using IdentityServer4.Models;

namespace Haus.Identity.Web.ApiResources.Commands
{
    public class CreateApiResourceCommand : ICommand
    {
        public string Identifier { get; }
        public string[] Scopes { get; }
        public string DisplayName { get; }

        public CreateApiResourceCommand(string identifier, string[] scopes, string displayName = null)
        {
            Identifier = identifier;
            Scopes = scopes;
            DisplayName = displayName;
        }

        public ApiResource ToApiResource()
        {
            return new ApiResource(Identifier)
            {
                DisplayName = DisplayName ?? Identifier,
                Scopes = Scopes
            };
        }

        public ApiScope[] ToApiScopes()
        {
            return Scopes
                .Select(s => new ApiScope(s, s))
                .ToArray();
        }
    }
}