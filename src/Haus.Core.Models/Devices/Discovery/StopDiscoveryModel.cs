using Haus.Core.Models.ExternalMessages;

namespace Haus.Core.Models.Devices.Discovery
{
    public class StopDiscoveryModel
    {
        public const string Type = "stop_discovery";

        public HausCommand<StopDiscoveryModel> AsHausCommand()
        {
            return new HausCommand<StopDiscoveryModel>(Type, this);
        }
    }
}