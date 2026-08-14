using FluentValidation;
using Haus.Core.Models.Devices;

namespace Haus.Core.Devices.Validators;

public class DeviceModelValidator : AbstractValidator<DeviceModel>
{
    public DeviceModelValidator()
    {
        // DeviceModel.Name is nullable (unlike RoomModel.Name/MetadataModel.Key/Value), so the
        // shared Required() extension -- typed for non-nullable string -- doesn't fit here.
        RuleFor(d => d.Name).NotNull().NotEmpty();
        RuleForEach(d => d.Metadata).SetValidator(new DeviceMetadataModelValidator());
    }
}
