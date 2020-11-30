using System.Threading.Tasks;
using FluentValidation;
using FluentValidation.TestHelper;
using Haus.Core.Devices.Validators;
using Haus.Core.Models.Devices;
using Xunit;

namespace Haus.Core.Tests.Devices.Validators
{
    public class DeviceMetadataModelValidatorTests
    {
        private readonly IValidator<DeviceMetadataModel> _validator;

        public DeviceMetadataModelValidatorTests()
        {
            _validator = new DeviceMetadataModelValidator();
        }

        [Fact]
        public async Task WhenMetadataHasNullKeyThenReturnsInvalid()
        {
            var result = await _validator.TestValidateAsync(new DeviceMetadataModel(null, "something"));
            Assert.False(result.IsValid);
        }
        
        [Fact]
        public async Task WhenMetadataHasEmptyKeyThenReturnsInvalid()
        {
            var result = await _validator.TestValidateAsync(new DeviceMetadataModel(string.Empty, "something"));
            Assert.False(result.IsValid);
        }

        [Fact]
        public async Task WhenMetadataValueIsNullThenReturnsInvalid()
        {
            var result = await _validator.TestValidateAsync(new DeviceMetadataModel("something", null));
            Assert.False(result.IsValid);
        }

        [Fact]
        public async Task WhenMetadataValueIsEmptyThenReturnsInvalid()
        {
            var result = await _validator.TestValidateAsync(new DeviceMetadataModel("something", string.Empty));
            Assert.False(result.IsValid);
        }

        [Fact]
        public async Task WhenMetadataKeyAndValueIsFilledInThenReturnsValid()
        {
            var result = await _validator.TestValidateAsync(new DeviceMetadataModel("something", "idk"));
            Assert.True(result.IsValid);
        }
    }
}