using Haus.Zigbee.Serial.Frames;

namespace Haus.Zigbee.Connection;

// Data-only payload for the poll loop's indication-received event: it carries the decoded
// received-data frame and nothing else, keeping the event's data separate from the poll
// loop's handling logic.
public record ApsIndicationReceived(ApsDataIndicationFrame Indication);
