using Microsoft.Extensions.Configuration;

namespace Haus.Portal.Web.Settings
{
    public class SettingsModel
    {
        public string Authority { get; set; }
        public string ClientId { get; set; }
        public string ResponseType { get; set; }

        public static SettingsModel FromConfig(IConfiguration config)
        {
            return new SettingsModel
            {
                Authority = config.GetValue<string>("Auth:Authority"),
                ClientId = config.GetValue<string>("Auth:ClientId"),
                ResponseType = config.GetValue<string>("Auth:ResponseType")
            };
        }
    }
}