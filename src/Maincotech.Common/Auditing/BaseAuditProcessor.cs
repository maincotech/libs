using Maincotech.Logging;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Maincotech.Auditing
{
    public class BaseAuditProcessor : IAuditProcessor
    {
        private static readonly ILogger Logger = AppRuntimeContext.Current.GetLogger<BaseAuditProcessor>();

        public List<Type> AuditTypes { get; private set; }

        public BaseAuditProcessor()
        {
            AuditTypes = new List<Type>();
            var types = AppRuntimeContext.Current.TypeFinder.ClassesWithAttribute<AuditAttribute>();
            if (types.Any())
            {
                AuditTypes.AddRange(types);
            }
        }

        public virtual void Audit(AuditRecord auditRecord)
        {
            Logger.Info("{0}", auditRecord);
        }
    }
}