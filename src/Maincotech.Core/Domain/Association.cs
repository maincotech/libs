using System;

namespace Maincotech.Domain
{
    public class AssociationBase<TSource,TTarget>: IEntity
    {
        public  string Relation { get; set; }
        public Guid SourceId { get; set; }
        public TSource Source { get; set; }
        public Guid TargetId { get; set; }
        public TTarget Target { get; set; }

        public Guid Id { get; set; }
    }
}
