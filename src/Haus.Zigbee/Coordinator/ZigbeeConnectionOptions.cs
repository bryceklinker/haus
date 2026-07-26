namespace Haus.Zigbee.Coordinator;

// Configuration needed to open the serial line to the deCONZ coordinator. The baud rate defaults
// to the value deCONZ firmware ships with, so only the port name has to be supplied.
public class ZigbeeConnectionOptions
{
    private const int DeconzDefaultBaudRate = 38400;

    public string SerialPort { get; set; } = "";

    public int BaudRate { get; set; } = DeconzDefaultBaudRate;
}
