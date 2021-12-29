using FluentValidation;
using Haus.Core.Models.Devices;

namespace Haus.Core.Devices.Validators;

public class DeviceModelValidator : AbstractValidator<DeviceModel>
{
    public DeviceModelValidator()
    {
        RuleFor(d => d.Name).Required();
        RuleForEach(d => d.Metadata)
            .SetValidator(new DeviceMetadataModelValidator());
    }
}