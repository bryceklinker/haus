namespace Haus.Core.Common.Entities
{
    public interface IEntity
    {
        long Id { get; }
    }
    public abstract class Entity : IEntity
    {
        public long Id { get; set; }
    }
}