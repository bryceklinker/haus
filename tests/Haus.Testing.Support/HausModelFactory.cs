using Bogus;
using Haus.Core.Models.Devices;
using Haus.Core.Models.Rooms;

namespace Haus.Testing.Support;

public static class HausModelFactory
{
    private static readonly Faker Faker = new Faker();
    
    public static DeviceModel DeviceModel()
    {
        return new DeviceModel(
            Id: Faker.Random.Number(),
            RoomId: null,
            Name: Faker.Company.CompanyName(),
            ExternalId: Faker.Random.Uuid().ToString(),
            DeviceType: Faker.PickRandom<DeviceType>(),
            LightType: Faker.PickRandom<LightType>(),
            Metadata: [],
            Lighting: null
        );
    }

    public static RoomModel RoomModel()
    {
        return new RoomModel(
            Id: Faker.Random.Number(),
            Name: Faker.Company.CompanyName(),
            Lighting: null
        );
    }
}