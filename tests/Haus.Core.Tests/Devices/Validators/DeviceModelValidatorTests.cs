using System.Threading.Tasks;
using FluentAssertions;
using FluentValidation;
using FluentValidation.TestHelper;
using Haus.Core.Devices.Validators;
using Haus.Core.Models.Common;
using Haus.Core.Models.Devices;
using Xunit;

namespace Haus.Core.Tests.Devices.Validators;

public class DeviceModelValidatorTests
{
    private readonly IValidator<DeviceModel> _validator = new DeviceModelValidator();

    [Fact]
    public async Task WhenNameIsNullThenReturnsInvalid()
    {
        var result = await _validator.TestValidateAsync(new DeviceModel { Name = null });
        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public async Task WhenNameIsEmptyThenReturnsInvalid()
    {
        var result = await _validator.TestValidateAsync(new DeviceModel { Name = string.Empty });
        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public async Task WhenMetadataHasNullKeyThenReturnsInvalid()
    {
        var model = new DeviceModel(Name: "one", Metadata: [new MetadataModel(null!, null!)]);

        var result = await _validator.TestValidateAsync(model);

        result.IsValid.Should().BeFalse();
    }
}
