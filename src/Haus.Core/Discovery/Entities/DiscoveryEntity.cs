using System;
using System.Linq.Expressions;
using Haus.Core.Common.Entities;
using Haus.Core.Models.Discovery;

namespace Haus.Core.Discovery.Entities;

public record DiscoveryEntity : Entity
{
    public static readonly Expression<Func<DiscoveryEntity, DiscoveryModel>> ToModelExpression =
        d => new DiscoveryModel(d.State);

    public static readonly Lazy<Func<DiscoveryEntity, DiscoveryModel>> ToModelFunc = new(ToModelExpression.Compile);
    public DiscoveryState State { get; set; }

    public DiscoveryEntity()
        : this(0) { }

    public DiscoveryEntity(long id = 0, DiscoveryState state = DiscoveryState.Disabled)
    {
        Id = id;
        State = state;
    }

    public DiscoveryModel ToModel()
    {
        return ToModelFunc.Value.Invoke(this);
    }

    public void Start()
    {
        State = DiscoveryState.Enabled;
    }

    public void Stop()
    {
        State = DiscoveryState.Disabled;
    }
}
