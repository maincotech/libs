using System;

namespace Maincotech.Domain
{
    public interface IAssociation<TSource,TTarget>
    {
        public string Relation { get; set; }
        public Guid SourceId { get; set; }
        public TSource Source { get; set; }
        public Guid TargetId { get; set; }
        public TTarget Target { get; set; }
    }
}
