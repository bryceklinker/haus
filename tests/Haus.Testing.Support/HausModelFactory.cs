using Bogus;
using Haus.Core.Models.Common;
using Haus.Core.Models.Devices;
using Haus.Core.Models.Lighting;
using Haus.Core.Models.Rooms;

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
}
