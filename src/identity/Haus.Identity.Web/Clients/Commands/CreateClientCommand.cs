using System;
using System.Collections.Generic;
using System.Linq;
using Haus.Cqrs.Commands;
using Haus.Identity.Models.Clients;
using IdentityServer4.Models;

namespace Haus.Identity.Web.Clients.Commands
{
    public class CreateClientCommand : ICommand<CreateClientResult>
    {
        public string Name { get; }
        public bool GenerateSecret { get; }
        public string RedirectUri { get; }
        public string[] Scopes { get; }
        public string Secret { get; }
        public string ClientId { get; }

        public string RedirectUriOrigin
        {
            get
            {
                if (string.IsNullOrWhiteSpace(RedirectUri))
                    return null;

                var uri = new Uri(RedirectUri);
                var port = uri.IsDefaultPort
                    ? string.Empty
                    : $":{uri.Port}";
                return $"{uri.Scheme}://{uri.Host}{port}";
            }
        }

        public CreateClientCommand(string name,
            bool generateSecret = true,
            string redirectUri = null,
            string[] scopes = null)
        {
            Name = name;
            GenerateSecret = generateSecret;
            RedirectUri = redirectUri;
            Scopes = scopes ?? Array.Empty<string>();
            ClientId = $"{Guid.NewGuid()}";
            Secret = GenerateSecret ? $"{Guid.NewGuid()}" : null;
        }

        public CreateClientCommand(CreateClientPayload payload)
            : this(payload?.Name, redirectUri: payload?.RedirectUrl, scopes: payload?.Scopes)
        {
        }

        public Client ToClient()
        {
            return new Client
            {
                ClientId = ClientId,
                ClientName = string.IsNullOrWhiteSpace(Name) ? ClientId : Name,
                RequireClientSecret = false,
                AllowedGrantTypes = ClientDefaults.DefaultGrantTypes,
                AllowedScopes = ClientDefaults.DefaultScopes.Concat(Scopes).ToArray(),
                AllowedCorsOrigins = string.IsNullOrWhiteSpace(RedirectUriOrigin)
                    ? new List<string>()
                    : new List<string> {RedirectUriOrigin},

                ClientSecrets = GenerateSecret
                    ? new List<Secret> {new Secret(Secret.Sha256())}
                    : new List<Secret>(),

                RedirectUris = string.IsNullOrWhiteSpace(RedirectUri)
                    ? new List<string>()
                    : new List<string> {RedirectUri}
            };
        }

        public CreateClientResult ToResult()
        {
            return new CreateClientResult(ClientId, Secret);
        }
    }
}