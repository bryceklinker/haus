using System;

namespace Haus.Core.Models.Zigbee;

public record ZigbeeActivityEntryModel(string EventType, DateTimeOffset OccurredAt, object Payload);
