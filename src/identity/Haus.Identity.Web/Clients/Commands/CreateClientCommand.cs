using System;
using System.Collections.Generic;
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
        public string Secret { get; }
        public string ClientId { get; }

        public CreateClientCommand(string name, 
            bool generateSecret = true,
            string redirectUri = null)
        {
            Name = name;
            GenerateSecret = generateSecret;
            RedirectUri = redirectUri;
            ClientId = $"{Guid.NewGuid()}";
            Secret = GenerateSecret ? $"{Guid.NewGuid()}" : null;
        }

        public CreateClientCommand(CreateClientPayload payload)
            : this(payload.Name)
        {
            
        }

        public Client ToClient()
        {
            return new Client
            {
                ClientId = ClientId,
                ClientName = string.IsNullOrWhiteSpace(Name) ? ClientId : Name,
                AllowedGrantTypes = ClientDefaults.DefaultGrantTypes,
                AllowedScopes = ClientDefaults.DefaultScopes,
                AllowedCorsOrigins = new List<string>{"*"},
                
                ClientSecrets = GenerateSecret
                    ? new List<Secret> {new Secret(Secret.Sha256())}
                    : new List<Secret>(),
                
                RedirectUris = string.IsNullOrWhiteSpace(RedirectUri)
                    ? new List<string>()
                    : new List<string>{RedirectUri}
            };
        }

        public CreateClientResult ToResult()
        {
            return new CreateClientResult(ClientId, Secret);
        }
    }
}