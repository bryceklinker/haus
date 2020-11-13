using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace Haus.Zigbee.Host.Tests.Mappers
{
    public static class Zigbee2MqttMessages
    {
        public static byte[] InterviewSuccessful(
            string friendlyName,
            string description = "",
            string model = "",
            bool supported = true,
            string vendor = "")
        {
            var jObject = InterviewSuccessfulJObject(friendlyName, description, model, supported, vendor);
            return Encoding.UTF8.GetBytes(jObject.ToString(Formatting.None));
        }

        public static JObject InterviewSuccessfulJObject(
            string friendlyName, 
            string description = "",
            string model = "",
            bool supported = true,
            string vendor = "")
        {
            return JObject.FromObject(new
            {
                type = "pairing",
                message = "interview_successful",
                meta = new
                {
                    description = string.IsNullOrWhiteSpace(description) ? null : description,
                    friendly_name = friendlyName,
                    model = string.IsNullOrWhiteSpace(model) ? null : model,
                    supported,
                    vendor = string.IsNullOrWhiteSpace(vendor) ? null : vendor
                }
            });
        }
    }
}