namespace Haus.Identity.Models.Clients
{
    public class CreateClientPayload
    {
        public string Name { get; set; }
        public string[] Scopes { get; set; }
        public string RedirectUrl { get; set; }

        public CreateClientPayload(string name, string redirectUrl, string[] scopes)
        {
            Name = name;
            RedirectUrl = redirectUrl;
            Scopes = scopes;
        }
    }
}