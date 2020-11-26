using System;

namespace Haus.Core.Common
{
    public class EntityNotFoundException : Exception
    {
        public EntityNotFoundException(Type entityType, object id)
            : base($"No entity of type ${entityType.Name} found with id {id}.")
        {
            
        }
    }

    public class EntityNotFoundException<T> : EntityNotFoundException
    {
        public EntityNotFoundException(object id) 
            : base(typeof(T), id)
        {
        }
    }
}