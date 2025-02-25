namespace Haus.Utilities.Tests.TypeScript.GenerateModels.SampleModels;

public class GenericType<T>
{
    public T Item { get; set; }
}

public class GenericType<T, R, U>
{
    public T First { get; set; }
    public R Second { get; set; }
    public U Third { get; set; }
}
