using Haus.Zigbee.Coordinator;
using Xunit;

namespace Haus.Zigbee.Tests.Coordinator;

public class CommandRetryOptionsTests
{
    [Fact]
    public void DefaultOptions_HasSensibleDefaults()
    {
        var options = new CommandRetryOptions();

        Assert.Equal(3, options.MaxRetries);
        Assert.Equal(100, options.BaseBackoffMs);
        Assert.Equal(5000, options.MaxBackoffMs);
    }

    [Fact]
    public void Options_CanBeCustomized()
    {
        var options = new CommandRetryOptions
        {
            MaxRetries = 5,
            BaseBackoffMs = 200,
            MaxBackoffMs = 10000,
        };

        Assert.Equal(5, options.MaxRetries);
        Assert.Equal(200, options.BaseBackoffMs);
        Assert.Equal(10000, options.MaxBackoffMs);
    }
}
