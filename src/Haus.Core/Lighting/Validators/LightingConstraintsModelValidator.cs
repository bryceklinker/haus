using FluentValidation;
using Haus.Core.Models.Lighting;

namespace Haus.Core.Lighting.Validators
{
    public class LightingConstraintsModelValidator : AbstractValidator<LightingConstraintsModel>
    {
        public LightingConstraintsModelValidator()
        {
            RuleFor(c => c.MinBrightnessValue)
                .LessThanOrEqualTo(c => c.MaxBrightnessValue);

            RuleFor(c => c.MinTemperature)
                .LessThanOrEqualTo(c => c.MaxTemperature);
        }   
    }
}