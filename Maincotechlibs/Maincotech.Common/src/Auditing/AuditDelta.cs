using System;

namespace Maincotech.Auditing
{
    [Serializable]
    public class AuditDelta
    {
        public string FieldName { get; set; }
        public Type FieldType { get; set; }
        public object ValueBefore { get; set; }
        public object ValueAfter { get; set; }
    }
}