using System.Threading.Tasks;
using FluentAssertions;
using FluentValidation;
using FluentValidation.TestHelper;
using Haus.Core.Lighting.Validators;
using Haus.Core.Models.Lighting;
using Xunit;

namespace Haus.Core.Tests.Lighting.Validators
{
    public class LightingConstraintsModelValidatorTests
    {
        private readonly LightingConstraintsModelValidator _validator;

        public LightingConstraintsModelValidatorTests()
        {
            _validator = new LightingConstraintsModelValidator();
        }

        [Fact]
        public async Task WhenMinBrightnessIsGreaterThanMaxBrightnessThenReturnsInvalid()
        {
            var model = new LightingConstraintsModel(100, 0);
            var result = await _validator.TestValidateAsync(model);

            result.IsValid.Should().BeFalse();
        }

        [Fact]
        public async Task WhenMinTemperatureIsGreaterThanMaxTemperatureThenReturnsInvalid()
        {
            var model = new LightingConstraintsModel(MinTemperature: 9000, MaxTemperature: 4000);

            var result = await _validator.TestValidateAsync(model);

            result.IsValid.Should().BeFalse();
        }

        [Fact]
        public async Task WhenMinValuesAreLessThanMaxValuesThenReturnsValid()
        {
            var model = new LightingConstraintsModel(10, 25, 100, 200);

            var result = await _validator.TestValidateAsync(model);

            result.IsValid.Should().BeTrue();
        }
    }
}