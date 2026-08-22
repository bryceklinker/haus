namespace Haus.Zigbee.Models;

public record ZigbeeTransportError(string ErrorType, string Message, ushort? NetworkAddress, IeeeAddress? IeeeAddress);
