using System.Threading.Tasks;
using Haus.Core.Devices.Repositories;
using Haus.Core.Models.Devices;
using Haus.Core.Models.Devices.Sensors.Battery;
using Haus.Core.Models.Devices.Sensors.Light;
using Haus.Core.Models.Devices.Sensors.Temperature;
using Haus.Testing.Support;
using Haus.Web.Host.Tests.Support;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Haus.Web.Host.Tests.Devices;

[Collection(HausWebHostCollectionFixture.Name)]
public class SensorChangedTests(HausWebHostApplicationFactory factory)
{
    [Fact]
    public async Task WhenIlluminanceChangedForKnownDeviceThenDeviceIlluminanceIsPersisted()
    {
        var device = await factory.WaitForDeviceToBeDiscovered(DeviceType.LightSensor);

        await factory.PublishHausEventAsync(new IlluminanceChangedModel(device.ExternalId, 123, 45));

        await Eventually.AssertAsync(async () =>
        {
            using var scope = factory.Services.CreateScope();
            var repository = scope.ServiceProvider.GetRequiredService<IDeviceCommandRepository>();
            var entity = await repository.GetById(device.Id);
            Assert.Equal(123, entity.Illuminance);
            Assert.Equal(45, entity.Lux);
        });
    }

    [Fact]
    public async Task WhenTemperatureChangedForKnownDeviceThenDeviceTemperatureIsPersisted()
    {
        var device = await factory.WaitForDeviceToBeDiscovered(DeviceType.TemperatureSensor);

        await factory.PublishHausEventAsync(new TemperatureChangedModel(device.ExternalId, 21.5));

        await Eventually.AssertAsync(async () =>
        {
            using var scope = factory.Services.CreateScope();
            var repository = scope.ServiceProvider.GetRequiredService<IDeviceCommandRepository>();
            var entity = await repository.GetById(device.Id);
            Assert.Equal(21.5, entity.Temperature);
        });
    }

    [Fact]
    public async Task WhenBatteryChangedForKnownDeviceThenDeviceBatteryLevelIsPersisted()
    {
        var device = await factory.WaitForDeviceToBeDiscovered(DeviceType.MotionSensor);

        await factory.PublishHausEventAsync(new BatteryChangedModel(device.ExternalId, 80));

        await Eventually.AssertAsync(async () =>
        {
            using var scope = factory.Services.CreateScope();
            var repository = scope.ServiceProvider.GetRequiredService<IDeviceCommandRepository>();
            var entity = await repository.GetById(device.Id);
            Assert.Equal(80, entity.BatteryLevel);
        });
    }
}
