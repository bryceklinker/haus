using Haus.Core.Models.Common;
using Haus.Core.Models.ExternalMessages;

namespace Haus.Core.Models.Devices.Sensors.Battery;

public record BatteryChangedModel(string DeviceId, long BatteryLevel) : IHausEventCreator<BatteryChangedModel>
{
    public const string Type = "battery_changed";

    public HausEvent<BatteryChangedModel> AsHausEvent()
    {
        return new HausEvent<BatteryChangedModel>(Type, this);
    }
}
