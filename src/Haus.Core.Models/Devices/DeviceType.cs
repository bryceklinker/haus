using System;
using System.Text.Json.Serialization;

namespace Haus.Core.Models.Devices
{
    [Flags]
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum DeviceType
    {
        Unknown = 0,
        Light = 1,
        LightSensor = 2,
        MotionSensor = 4,
        TemperatureSensor = 8,
        Switch = 16
    }
}