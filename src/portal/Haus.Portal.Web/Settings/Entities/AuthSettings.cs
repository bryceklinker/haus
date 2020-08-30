namespace Haus.Portal.Web.Settings.Entities
{
    public class AuthSettings
    {
        public const string ClientName = "Haus Portal";
        public int Id { get; set; }
        public string Authority { get; set; }
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
    }
}