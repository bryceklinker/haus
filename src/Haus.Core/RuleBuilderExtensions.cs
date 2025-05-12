using FluentValidation;

namespace Haus.Core;

public static class RuleBuilderExtensions
{
    public static IRuleBuilder<T, string> Required<T>(this IRuleBuilder<T, string> builder)
    {
        return builder.NotNull().NotEmpty();
    }
}
