namespace Haus.Zigbee.Coordinator;

public record ZigbeeConnectionStatus(bool IsConnected, NetworkConfig? NetworkConfig, string? Reason);
