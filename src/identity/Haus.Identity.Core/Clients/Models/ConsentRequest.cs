using System.Linq;

namespace Haus.Identity.Core.Clients.Models
{
    public class ConsentRequest
    {
        public string ClientId { get; set; }
        public string ClientName { get; set; }
        public string ReturnUrl { get; set; }
        public bool DidConsent { get; set; }
        public ConsentScopeRequest[] IdentityScopes { get; set; }
        public ConsentScopeRequest[] ResourceScopes { get; set; }

        public ConsentScopeRequest[] AllScopes => IdentityScopes.Union(ResourceScopes).ToArray();
        public string[] ConsentedScopes => AllScopes
            .Select(i => i.Name)
            .ToArray();
    }
}