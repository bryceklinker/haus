namespace Haus.Utilities.Tests.TypeScript.GenerateModels.SampleModels;

public class GenericType<T>
{
    public T? Item { get; set; }
}

public class GenericType<T, TR, TU>
{
    public T? First { get; set; }
    public TR? Second { get; set; }
    public TU? Third { get; set; }
}
