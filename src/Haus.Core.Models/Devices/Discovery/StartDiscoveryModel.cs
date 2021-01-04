using Haus.Core.Models.Common;
using Haus.Core.Models.ExternalMessages;

namespace Haus.Core.Models.Devices.Discovery
{
    public record StartDiscoveryModel : IHausCommandCreator<StartDiscoveryModel>
    {
        public const string Type = "start_discovery";

        public HausCommand<StartDiscoveryModel> AsHausCommand() => new(Type, this);
    }
}