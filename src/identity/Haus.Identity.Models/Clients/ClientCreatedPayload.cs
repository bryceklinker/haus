namespace Haus.Identity.Models.Clients
{
    public class ClientCreatedPayload
    {
        public string ClientName { get; set; }
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }

        public ClientCreatedPayload(string clientName, string clientId, string clientSecret)
        {
            ClientName = clientName;
            ClientId = clientId;
            ClientSecret = clientSecret;
        }
    }
}