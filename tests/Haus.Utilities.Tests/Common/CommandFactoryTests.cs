using Haus.Utilities.Common.Cli;
using Haus.Utilities.Zigbee2Mqtt.Commands;
using Microsoft.Extensions.Logging.Abstractions;
using Xunit;

namespace Haus.Utilities.Tests.Common;

public class CommandFactoryTests
{
    private readonly CommandFactory _commandFactory = new(new NullLogger<CommandFactory>());

    [Fact]
    public void WhenCommandGroupAndNameMatchesCommandThenReturnsAnInstanceOfTheMatchingCommand()
    {
        var command = _commandFactory.Create(["zigbee2mqtt", "generate-device-type-defaults"]);

        Assert.IsType<GenerateDefaultDeviceTypeOptionsCommand>(command);
    }

    [Fact]
    public void WhenNoCommandMatchesThenThrowsCommandNotFoundException()
    {
        var args = new[] { "one", "two" };
        Assert.Throws<CommandNotFoundException>(() => _commandFactory.Create(args));
    }
}
