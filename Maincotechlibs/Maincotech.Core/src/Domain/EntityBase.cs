using System;

namespace Maincotech.Domain
{
    public class EntityBase : IEntity
    {
        public Guid Id { get; set; }
    }
}