using System;

namespace Haus.Identity.Core.Clients.Models
{
    public class CreateClientRequest
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string[] Scopes { get; set; } = Array.Empty<string>();
    }
}