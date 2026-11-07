using System;

namespace Haus.Zigbee.Models.Diagnostics;

public record ZigbeeActivityEntryModel(string EventType, DateTimeOffset OccurredAt, object Payload);
