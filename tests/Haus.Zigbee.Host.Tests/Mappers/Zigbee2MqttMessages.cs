using System.Text;
using System.Text.Json;

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
            return JsonSerializer.SerializeToUtf8Bytes(new
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