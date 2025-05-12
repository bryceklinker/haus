using System.Threading.Tasks;
using FluentAssertions;
using FluentValidation.TestHelper;
using Haus.Core.Common.Storage;
using Haus.Core.Models.Rooms;
using Haus.Core.Rooms.Validators;
using Haus.Testing.Support;
using Xunit;

namespace Haus.Core.Tests.Rooms.Validators;

public class RoomModelValidatorTests
{
    private readonly HausDbContext _context;
    private readonly RoomModelValidator _validator;

    public RoomModelValidatorTests()
    {
        _context = HausDbContextFactory.Create();
        _validator = new RoomModelValidator(_context);
    }

    [Fact]
    public async Task WhenNameIsMissingThenReturnsInvalid()
    {
        var result = await _validator.TestValidateAsync(new RoomModel(Name: null!));

        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public async Task WhenNameIsDuplicateThenReturnsInvalid()
    {
        _context.AddRoom("one");

        var result = await _validator.TestValidateAsync(new RoomModel(Name: "one"));

        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public async Task WhenNameIsDuplicateWithTheSameIdThenReturnsValid()
    {
        var room = _context.AddRoom("three");

        var result = await _validator.TestValidateAsync(new RoomModel(room.Id, "three"));

        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public async Task WhenNameIsUniqueThenReturnsValid()
    {
        _context.AddRoom("one");

        var result = await _validator.TestValidateAsync(new RoomModel(Name: "two"));

        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public async Task WhenOccupancyTimeoutIsNegativeThenReturnsInvalid()
    {
        var model = new RoomModel(Name: "one", OccupancyTimeoutInSeconds: -1);

        var result = await _validator.TestValidateAsync(model);

        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public async Task WhenRequiredPropertiesAreProvidedThenReturnsTrue()
    {
        var model = new RoomModel(Name: "bob", OccupancyTimeoutInSeconds: 3);

        var result = await _validator.TestValidateAsync(model);

        result.IsValid.Should().BeTrue();
    }
}
