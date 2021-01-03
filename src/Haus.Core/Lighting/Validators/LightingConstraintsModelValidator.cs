using FluentValidation;
using Haus.Core.Models.Lighting;

namespace Haus.Core.Lighting.Validators
{
    public class LightingConstraintsModelValidator : AbstractValidator<LightingConstraintsModel>
    {
        public LightingConstraintsModelValidator()
        {
            RuleFor(c => c.MinLevel)
                .LessThanOrEqualTo(c => c.MaxLevel);

            RuleFor(c => c.MinTemperature)
                .LessThanOrEqualTo(c => c.MaxTemperature);
        }   
    }
}