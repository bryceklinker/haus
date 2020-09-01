using System.Text.Json.Serialization;

namespace Haus.Portal.Web.Settings
{
    public class SettingsModel
    {
        public string Authority { get; set; }
        
        [JsonPropertyName("client_id")]
        public string ClientId { get; set; }
        
        [JsonPropertyName("response_type")]
        public string ResponseType { get; set; }
        
        public string Scope { get; set; }
    }
}