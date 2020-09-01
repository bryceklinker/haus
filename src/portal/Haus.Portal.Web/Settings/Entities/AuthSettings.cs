namespace Haus.Portal.Web.Settings.Entities
{
    public class AuthSettings
    {
        public const string ApiResourceIdentifier = "portal";
        public const string ClientName = "Haus Portal";
        public static readonly string[] Scopes = {"portal", "portal/read", "portal/write"};
        public int Id { get; set; }
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
    }
}