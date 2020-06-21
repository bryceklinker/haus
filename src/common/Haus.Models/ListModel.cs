using System.Collections.Generic;
using System.Collections.Immutable;

namespace Haus.Models
{
    public class ListModel<TItem>
    {
        public ImmutableArray<TItem> Items { get; }

        public ListModel(IEnumerable<TItem> items)
        {
            Items = items.ToImmutableArray();
        }
    }
}