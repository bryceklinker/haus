using System;
using System.Collections.Generic;
using System.Linq;
using Haus.Identity.Core.Clients.Models;
using IdentityServer4.Models;

namespace Haus.Identity.Core.Clients.Factories
{
    public interface IConsentRequestFactory
    {
        ConsentRequest Create(Client client,
            string returnUrl,
            IEnumerable<IdentityResource> identityResources = null, 
            IEnumerable<ApiResource> apiResources = null);
    }

    public class ConsentRequestFactory : IConsentRequestFactory
    {
        public ConsentRequest Create(Client client,
            string returnUrl,
            IEnumerable<IdentityResource> identityResources = null, 
            IEnumerable<ApiResource> apiResources = null)
        {
            
            return new ConsentRequest
            {
                ClientId = client.ClientId,
                ClientName = client.ClientName,
                ReturnUrl = returnUrl,
                IdentityScopes = (identityResources ?? Array.Empty<IdentityResource>())
                    .Select(CreateScopeRequest)
                    .ToArray(),
                ResourceScopes = (apiResources ?? Array.Empty<ApiResource>())
                    .SelectMany(r => r.Scopes ?? Array.Empty<Scope>())
                    .Select(CreateScopeRequest)
                    .ToArray()
            };
        }

        private ConsentScopeRequest CreateScopeRequest(IdentityResource identityResource)
        {
            return new ConsentScopeRequest
            {
                Name = identityResource.Name,
                DisplayName = identityResource.DisplayName,
                Emphasize = identityResource.Emphasize,
                Required = identityResource.Required
            };
        }

        private ConsentScopeRequest CreateScopeRequest(Scope scope)
        {
            return new ConsentScopeRequest
            {
                Name = scope.Name,
                DisplayName = scope.DisplayName,
                Emphasize = scope.Emphasize,
                Required = scope.Required
            };
        }
    }
}