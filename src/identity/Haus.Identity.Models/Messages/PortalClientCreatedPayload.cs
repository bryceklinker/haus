namespace Haus.Identity.Models.Messages
{
    public class PortalClientCreatedPayload
    {
        public const string Type = "[Identity] Portal Client Created";
        
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }

        public PortalClientCreatedPayload(string clientId, string clientSecret)
        {
            ClientId = clientId;
            ClientSecret = clientSecret;
        }
    }
}