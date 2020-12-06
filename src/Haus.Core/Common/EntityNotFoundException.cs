using System;
using System.Collections.Generic;
using Haus.Core.Common.Entities;

namespace Haus.Core.Common
{
    public class EntityNotFoundException : Exception
    {
        public EntityNotFoundException(Type entityType, long id)
            : base($"No entity of type ${entityType.Name} found with id {id}.")
        {
            
        }

        public EntityNotFoundException(Type entityType, IEnumerable<long> ids)
            : base($"No entities of type ${entityType.Name} found with ids {string.Join(", ", ids)}.")
        {
            
        }
    }

    public class EntityNotFoundException<TEntity> : EntityNotFoundException
        where TEntity : IEntity
    {
        public EntityNotFoundException(long id) 
            : base(typeof(TEntity), id)
        {
        }

        public EntityNotFoundException(IEnumerable<long> ids)
            : base(typeof(TEntity), ids)
        {
            
        }
    }
}