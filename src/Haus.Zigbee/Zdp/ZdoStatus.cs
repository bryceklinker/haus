namespace Haus.Zigbee.Zdp;

public enum ZdoStatus : byte
{
    Success = 0x00,
    InvalidRequestType = 0x80,
    DeviceNotFound = 0x81,
    InvalidEndpoint = 0x82,
    NotActive = 0x83,
    NotSupported = 0x84,
    Timeout = 0x85,
    NoMatch = 0x86,
    NoEntry = 0x88,
    NoDescriptor = 0x89,
    InsufficientSpace = 0x8a,
    NotPermitted = 0x8b,
    TableFull = 0x8c,
    NotAuthorized = 0x8d,
    InvalidIndex = 0x8f,
}
