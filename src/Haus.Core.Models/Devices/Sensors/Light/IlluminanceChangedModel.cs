using Haus.Core.Models.Common;
using Haus.Core.Models.ExternalMessages;

namespace Haus.Core.Models.Devices.Sensors.Light;

public record IlluminanceChangedModel(string DeviceId, long Illuminance, long? Lux)
    : IHausEventCreator<IlluminanceChangedModel>
{
    public const string Type = "illuminance_changed";

    public HausEvent<IlluminanceChangedModel> AsHausEvent()
    {
        return new HausEvent<IlluminanceChangedModel>(Type, this);
    }
}
