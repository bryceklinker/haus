using Haus.Core.Models.ExternalMessages;

namespace Haus.Core.Models.Devices.Discovery
{
    public class StartDiscoveryModel
    {
        public const string Type = "start_discovery";

        public HausCommand<StartDiscoveryModel> AsHausCommand()
        {
            return new HausCommand<StartDiscoveryModel>(Type, this);
        }
    }
}