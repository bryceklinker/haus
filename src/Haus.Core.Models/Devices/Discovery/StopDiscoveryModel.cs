using Haus.Core.Models.Common;
using Haus.Core.Models.ExternalMessages;

namespace Haus.Core.Models.Devices.Discovery
{
    public record StopDiscoveryModel : IHausCommandCreator<StopDiscoveryModel>
    {
        public const string Type = "stop_discovery";

        public HausCommand<StopDiscoveryModel> AsHausCommand() => new HausCommand<StopDiscoveryModel>(Type, this);
    }
}