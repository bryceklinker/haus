namespace Haus.Core.Common.Entities;

public interface IEntity
{
    long Id { get; }
}

public abstract record Entity : IEntity
{
    public long Id { get; set; }
}