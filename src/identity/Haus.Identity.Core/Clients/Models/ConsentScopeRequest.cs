namespace Haus.Identity.Core.Clients.Models
{
    public class ConsentScopeRequest
    {
        public string Name { get; set; }
        public string DisplayName { get; set; }
        public bool Required { get; set; }
        public bool Emphasize { get; set; }
    }
}