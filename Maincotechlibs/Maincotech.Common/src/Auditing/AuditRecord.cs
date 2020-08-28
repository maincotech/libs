using System;
using System.Collections.Generic;

namespace Maincotech.Auditing
{
    [Serializable]
    public class AuditRecord
    {
        public AuditRecord()
        {
            Changes = new List<AuditDelta>();
        }

        public List<AuditDelta> Changes { get; set; }
        public Type ObjectType { get; set; }
        public DateTime DateTimeStamp { get; set; }
        public OperationType OperationType { get; set; }
        public string UserName { get; set; }
        public string ObjectId { get; set; }
        public string From { get; set; }
    }
}
