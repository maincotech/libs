using System;
using System.Collections.Generic;

namespace Maincotech.Auditing
{
    public interface IAuditProcessor
    {
        List<Type> AuditTypes { get; }

        void Audit(AuditRecord auditRecord);
    }
}
