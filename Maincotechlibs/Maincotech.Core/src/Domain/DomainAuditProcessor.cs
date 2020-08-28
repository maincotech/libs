using Maincotech.Auditing;
using Maincotech.Domain.Repositories;
using Maincotech.Logging;
using Maincotech.Utilities;
using System;
using System.Collections.Generic;

namespace Maincotech.Domain
{
    public class DomainAuditProcessor : IAuditProcessor
    {
        private static readonly ILogger Logger = AppRuntimeContext.Current.GetLogger<DomainAuditProcessor>();

        private readonly IRepository<DomainObjectLifeCycle> _Repository;

        public DomainAuditProcessor(IRepository<DomainObjectLifeCycle> repository)
        {
            _Repository = repository;

            var types = AppRuntimeContext.Current.TypeFinder.ClassesOfType<IEntity>();
            foreach (var type in types)
            {
                if (type.GetCustomAttributes(typeof(AuditAttribute), true).Length > 0)
                {
                    AuditTypes.Add(type);
                }
            }
        }

        public List<Type> AuditTypes { get; private set; }

        public void Audit(AuditRecord auditRecord)
        {
            try
            {
                var entity = new DomainObjectLifeCycle
                {
                    Id = Guid.NewGuid(),
                    ObjectId = auditRecord.ObjectId,
                    ObjectType = auditRecord.ObjectType.FullName,
                    From = auditRecord.From,
                    OperationTime = auditRecord.DateTimeStamp,
                    UserName = auditRecord.UserName,
                    Detail = SerializerHelper.SerializeToBase64String(auditRecord.Changes),
                    OperationType = auditRecord.OperationType
                };
                if (_Repository == null)
                {
                    return;
                }
                _Repository.Context.AuditEnabled = false;
                _Repository.Add(entity);
                _Repository.Context.Commit();
            }
            catch (Exception ex)
            {
                Logger.Error("Failed to audit record {0}", ex, auditRecord);
            }
            finally
            {
                if (_Repository != null)
                {
                    _Repository.Context.AuditEnabled = true;
                }
            }
        }
    }
}