using FluentValidation;
using Haus.Core.Models.Common;
using Haus.Core.Models.Devices;

namespace Haus.Core.Devices.Validators
{
    public class DeviceMetadataModelValidator : AbstractValidator<MetadataModel>
    {
        public DeviceMetadataModelValidator()
        {
            RuleFor(m => m.Key).Required();
            RuleFor(m => m.Value).Required();
        }
    }
}