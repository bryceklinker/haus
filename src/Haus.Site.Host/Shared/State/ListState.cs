using System;

namespace Haus.Site.Host.Shared.State;

public abstract record ListState<T>(
    T[]? Items = null, 
    bool IsLoading = false,
    Exception? Error = null)
{
    public T[] Items { get; init; } = Items ?? [];
    
    protected ListState()
        : this([])
    {
        
    }
}