namespace Haus.Web.Host.Diagnostics.Models
{
    public class MqttDiagnosticsMessageModel
    {
        public string Id { get; set; }
        public string Topic { get; set; }
        public object Payload { get; set; }
    }
}