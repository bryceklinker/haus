namespace Haus.Zigbee.Coordinator;

public class CommandRetryOptions
{
    public int MaxRetries { get; set; } = 3;

    public int BaseBackoffMs { get; set; } = 100;

    public int MaxBackoffMs { get; set; } = 5000;
}
