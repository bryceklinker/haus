using System;

namespace Haus.Core.Models.Common;

public class ListResult<T>
{
    public T[] Items { get; set; }

    public int Count => Items.Length;

    public ListResult(T[] items)
    {
        Items = items ?? Array.Empty<T>();
    }
}