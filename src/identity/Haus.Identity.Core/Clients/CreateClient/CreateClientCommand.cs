using System;
using Haus.Identity.Core.Common.Messaging;
using Haus.Identity.Core.Common.Messaging.Commands;

namespace Haus.Identity.Core.Clients.CreateClient
{
    public class CreateClientCommand : ICommand<CreateClientResult>
    {
        public string Id { get; }
        public string Name { get; }
        public string[] AllowedCorsOrigins { get; }
        public string[] Scopes { get; }
        public string[] RedirectUris { get; }

        public CreateClientCommand(
            string id, 
            string name, 
            string[] scopes = null, 
            string[] redirectUris = null,
            string[] allowedCorsOrigins = null)
        {
            Id = id;
            Name = name;
            AllowedCorsOrigins = allowedCorsOrigins ?? Array.Empty<string>();
            Scopes = scopes ?? Array.Empty<string>();
            RedirectUris = redirectUris ?? Array.Empty<string>();
            
        }
    }
}