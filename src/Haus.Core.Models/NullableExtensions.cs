namespace Haus.Core.Models;

public static class NullableExtensions
{
    public static bool IsNull<T>(this T? value)
        where T : struct
    {
        return !value.HasValue;
    }
}
