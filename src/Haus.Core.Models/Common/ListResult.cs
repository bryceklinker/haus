using System;

namespace Haus.Core.Models.Common;

public class ListResult<T>(T[]? items = null)
{
    public T[] Items { get; set; } = items ?? [];

    public int Count => Items.Length;
}
