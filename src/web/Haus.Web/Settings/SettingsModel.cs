using System.Text.Json.Serialization;
using Microsoft.Extensions.Configuration;

namespace Haus.Web.Settings
{
    public class SettingsModel
    {
        public string Authority { get; set; }
        [JsonPropertyName("client_id")]
        public string ClientId { get; set; }
        public string Scope { get; set; }
        [JsonPropertyName("redirect_uri")]
        public string RedirectUri { get; set; }
        [JsonPropertyName("response_type")]
        public string ResponseType { get; set; }

        public static SettingsModel FromConfiguration(IConfiguration config)
        {
            return new SettingsModel
            {
                Authority = config.GetValue<string>("authority"),
                ClientId = config.GetValue<string>("client_id"),
                ResponseType = config.GetValue<string>("response_type"),
                Scope = config.GetValue<string>("scope"),
                RedirectUri = config.GetValue<string>("redirect_uri")
            };
        }
    }
}