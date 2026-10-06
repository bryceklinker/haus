using Haus.Core.Models.Common;
using Haus.Core.Models.ExternalMessages;

namespace Haus.Core.Models.Devices.Sensors.Temperature;

public record TemperatureChangedModel(string DeviceId, double Temperature) : IHausEventCreator<TemperatureChangedModel>
{
    public const string Type = "temperature_sensor_changed";

    public HausEvent<TemperatureChangedModel> AsHausEvent()
    {
        return new HausEvent<TemperatureChangedModel>(Type, this);
    }
}
