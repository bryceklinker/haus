using System;
using System.Text.Json.Serialization;

namespace Haus.Core.Models.Devices
{
    [Flags]
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum DeviceType
    {
        Unknown = 0,
        BatterySensor = 1,
        Light = 2,
        LightSensor = 3,
        MotionSensor = 4,
        TemperatureSensor = 5
    }
}