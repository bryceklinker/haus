namespace Haus.Zigbee.Coordinator;

// Configuration needed to reach the deCONZ coordinator. The baud rate defaults to the value
// deCONZ firmware ships with, so only the port name has to be supplied for real hardware.
// TcpHost is the escape hatch for environments with no physical dongle attached (a fake-dongle
// simulator, typically) -- when set, it takes precedence over SerialPort.
public class ZigbeeConnectionOptions
{
    private const int DeconzDefaultBaudRate = 38400;

    public string SerialPort { get; set; } = "";

    public int BaudRate { get; set; } = DeconzDefaultBaudRate;

    public string? TcpHost { get; set; }

    public int TcpPort { get; set; }
}
