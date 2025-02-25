using Haus.Core.Models.Common;
using Haus.Core.Models.ExternalMessages;

namespace Haus.Core.Models.Discovery;

public record StopDiscoveryModel : IHausCommandCreator<StopDiscoveryModel>
{
    public const string Type = "stop_discovery";

    public HausCommand<StopDiscoveryModel> AsHausCommand()
    {
        return new(Type, this);
    }
}
