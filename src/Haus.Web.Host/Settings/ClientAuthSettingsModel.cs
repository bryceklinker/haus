namespace Haus.Web.Host.Settings
{
    public class ClientAuthSettingsModel
    {
        public string Domain { get; set; }
        public string ClientId { get; set; }
        
        public string Audience { get; set; }
    }
}