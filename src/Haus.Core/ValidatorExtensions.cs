using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using Haus.Core.Common;

namespace Haus.Core;

public static class ValidatorExtensions
{
    public static async Task HausValidateAndThrowAsync<T>(
        this IValidator<T> validator,
        T instance,
        CancellationToken token = default
    )
    {
        try
        {
            await validator.ValidateAndThrowAsync(instance, token).ConfigureAwait(false);
        }
        catch (ValidationException exception)
        {
            throw new HausValidationException(exception.Message, exception.Errors);
        }
    }
}
