using System.Dynamic;
using Bogus;
using Haus.Core.Models.Common;
using Haus.Core.Models.Devices;
using Haus.Core.Models.DeviceSimulator;
using Haus.Core.Models.Health;
using Haus.Core.Models.Lighting;
using Haus.Core.Models.Logs;
using Haus.Core.Models.Rooms;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Logging;

namespace Haus.Testing.Support;

public static class HausModelFactory
{
    private static readonly Faker Faker = new Faker();

    public static DeviceModel DeviceModel()
    {
        return new DeviceModel(
            Id: Faker.Random.Long(),
            RoomId: null,
            Name: Faker.Company.CompanyName(),
            ExternalId: Faker.Random.Uuid().ToString(),
            DeviceType: Faker.PickRandom<DeviceType>(),
            LightType: Faker.PickRandom<LightType>(),
            Metadata: [],
            Lighting: null
        );
    }

    public static MetadataModel MetadataModel()
    {
        return new MetadataModel(Key: Faker.Random.Word(), Value: Faker.Random.Uuid().ToString("n"));
    }

    public static RoomModel RoomModel()
    {
        return new RoomModel(
            Id: Faker.Random.Long(),
            Name: Faker.Company.CompanyName(),
            OccupancyTimeoutInSeconds: Faker.Random.Int(),
            Lighting: null
        );
    }

    public static LightingModel LightingModel()
    {
        return new LightingModel(State: Faker.Random.Enum<LightingState>(), Level: new LevelLightingModel());
    }

    public static HausHealthReportModel HealthReportModel()
    {
        return new HausHealthReportModel(
            Status: Faker.Random.Enum<HealthStatus>(),
            DurationOfCheckInMilliseconds: Faker.Random.Double(),
            []
        );
    }

    public static HausHealthCheckModel HealthCheckModel()
    {
        return new HausHealthCheckModel(
            Name: Faker.Hacker.Noun(),
            Status: Faker.Random.Enum<HealthStatus>(),
            DurationOfCheckInMilliseconds: Faker.Random.Double()
        );
    }

    public static SimulatedDeviceModel SimulatedDeviceModel()
    {
        return new SimulatedDeviceModel(
            Id: $"{Faker.Random.Uuid()}",
            DeviceType: DeviceType.Unknown,
            IsOccupied: false,
            Lighting: null,
            Metadata: [Core.Models.Common.MetadataModel.Simulated()]
        );
    }

    public static DeviceSimulatorStateModel DeviceSimulatorStateModel()
    {
        return new DeviceSimulatorStateModel(Devices: []);
    }

    public static LogEntryModel LogEntryModel()
    {
        return new LogEntryModel(
            Timestamp: Faker.Date.Recent().ToUniversalTime().ToString("O"),
            Level: Faker.PickRandom<LogLevel>().ToString(),
            Message: Faker.Lorem.Sentence(),
            Value: new ExpandoObject()
        );
    }
}
