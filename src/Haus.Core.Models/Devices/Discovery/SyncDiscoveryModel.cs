using Haus.Core.Models.Common;
using Haus.Core.Models.ExternalMessages;

namespace Haus.Core.Models.Devices.Discovery
{
    public record SyncDiscoveryModel : IHausCommandCreator<SyncDiscoveryModel>
    {
        public const string Type = "sync_discovery";
        
        public HausCommand<SyncDiscoveryModel> AsHausCommand()
        {
            return new HausCommand<SyncDiscoveryModel>(Type, this);
        }
    }
}