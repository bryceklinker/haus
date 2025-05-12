using System.Threading.Tasks;
using FluentAssertions;
using FluentValidation;
using FluentValidation.TestHelper;
using Haus.Core.Devices.Validators;
using Haus.Core.Models.Common;
using Xunit;

namespace Haus.Core.Tests.Devices.Validators;

public class DeviceMetadataModelValidatorTests
{
    private readonly IValidator<MetadataModel> _validator = new DeviceMetadataModelValidator();

    [Fact]
    public async Task WhenMetadataHasNullKeyThenReturnsInvalid()
    {
        var result = await _validator.TestValidateAsync(new MetadataModel(null!, "something"));
        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public async Task WhenMetadataHasEmptyKeyThenReturnsInvalid()
    {
        var result = await _validator.TestValidateAsync(new MetadataModel(string.Empty, "something"));
        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public async Task WhenMetadataValueIsNullThenReturnsInvalid()
    {
        var result = await _validator.TestValidateAsync(new MetadataModel("something", null!));
        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public async Task WhenMetadataValueIsEmptyThenReturnsInvalid()
    {
        var result = await _validator.TestValidateAsync(new MetadataModel("something", string.Empty));
        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public async Task WhenMetadataKeyAndValueIsFilledInThenReturnsValid()
    {
        var result = await _validator.TestValidateAsync(new MetadataModel("something", "idk"));
        result.IsValid.Should().BeTrue();
    }
}
