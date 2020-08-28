using Maincotech.Auditing;
using System;

namespace Maincotech.Domain
{
    public class DomainObjectLifeCycle : EntityBase
    {
        public string ObjectId { get; set; }
        public string ObjectType { get; set; }
        public OperationType OperationType { get; set; }
        public string Detail { get; set; }
        public string UserName { get; set; }
        public DateTime OperationTime { get; set; }
        public string From { get; set; }
    }
}