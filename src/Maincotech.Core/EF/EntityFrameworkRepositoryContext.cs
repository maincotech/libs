using Maincotech.Auditing;
using Maincotech.Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Reflection;
using System.Threading;

namespace Maincotech.EF
{
    /// <summary>
    /// Represents the Entity Framework repository context.
    /// </summary>
    public class EntityFrameworkRepositoryContext : RepositoryContext, IEntityFrameworkRepositoryContext
    {
        private static BindingFlags flags = BindingFlags.Static | BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.IgnoreCase;

        #region Private Fields

        private readonly object _sync = new object();

        #endregion Private Fields

        #region Ctor

        /// <summary>
        /// Initializes a new instance of <c>EntityFrameworkRepositoryContext</c> class.
        /// </summary>
        /// <param name="efContext">The <see cref="DbContext"/> object that is used when initializing the <c>EntityFrameworkRepositoryContext</c> class.</param>
        public EntityFrameworkRepositoryContext(DbContext efContext)
        {
            Context = efContext;
        }

        #endregion Ctor

        #region Protected Methods

        /// <summary>
        /// Disposes the object.
        /// </summary>
        /// <param name="disposing">A <see cref="System.Boolean"/> value which indicates whether
        /// the object should be disposed explicitly.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                // The dispose method will no longer be responsible for the commit
                // handling. Since the object container might handle the lifetime
                // of the repository context on the WCF per-request basis, users should
                // handle the commit logic by themselves.
                //if (!committed)
                //{
                //    Commit();
                //}
                Context.Dispose();
            }
            base.Dispose(disposing);
        }

        protected override void DoCommit()
        {
            if (Committed) return;
            lock (_sync)
            {
                if (AuditEnabled)
                {
                    AudtiChanges();
                }
                Context.SaveChanges();
            }
            Committed = true;
        }

        #endregion Protected Methods

        #region IEntityFrameworkRepositoryContext Members

        /// <summary>
        /// Gets the <see cref="DbContext"/> instance handled by Entity Framework.
        /// </summary>
        public DbContext Context { get; }

        #endregion IEntityFrameworkRepositoryContext Members

        #region IRepositoryContext Members

        public override string Provider => Context.Database.ProviderName;

        /// <summary>
        /// Registers a deleted object to the repository context.
        /// </summary>
        /// <param name="obj">The object to be registered.</param>
        public override void RegisterDeleted<TAggregateRoot>(TAggregateRoot obj)
        {
            Context.Entry(obj).State = EntityState.Deleted;
            Committed = false;
        }

        /// <summary>
        /// Registers a modified object to the repository context.
        /// </summary>
        /// <param name="obj">The object to be registered.</param>
        public override void RegisterModified<TAggregateRoot>(TAggregateRoot obj)
        {
            var originalEntity = Context.Find(obj.GetType(), obj.Id);

            //if (Context.Entry(originalEntity).State == EntityState.Modified)
            //{
            //    Context.Update(obj);
            //}

            //Context.Entry(originalEntity).CurrentValues.SetValues(obj);

            if (Context.Entry(originalEntity).State == EntityState.Detached)
            {
                Context.Entry(originalEntity).State = EntityState.Modified;
            }
            else
            {
                Context.Entry(originalEntity).CurrentValues.SetValues(obj);
            }
            Committed = false;
        }

        public override void RegisterNew<TAggregateRoot>(TAggregateRoot obj)
        {
            Context.Entry(obj).State = EntityState.Added;
            Committed = false;
        }

        #endregion IRepositoryContext Members

        #region IUnitOfWork Members

        /// <summary>
        /// Gets a <see cref="System.Boolean"/> value which indicates
        /// whether the Unit of Work could support Microsoft Distributed
        /// Transaction Coordinator (MS-DTC).
        /// </summary>
        public override bool DistributedTransactionSupported
        {
            get { return true; }
        }

        /// <summary>
        /// Rollback the transaction.
        /// </summary>
        public override void Rollback()
        {
            Committed = false;
        }

        #endregion IUnitOfWork Members

        private void AudtiChanges()
        {
            var auditProcessor = AppRuntimeContext.Current.Resolve<IAuditProcessor>() as IAuditProcessor;
            if (auditProcessor != null)
            {
                var changeTrack = Context.ChangeTracker.Entries().Where(p => p.State == EntityState.Added || p.State == EntityState.Deleted || p.State == EntityState.Modified);
                foreach (var entry in changeTrack)
                {
                    var entityType = entry.Entity.GetType();
                    if (entityType.Namespace == "System.Data.Entity.DynamicProxies")
                    {
                        entityType = entityType.BaseType;
                    }
                    if (auditProcessor.AuditTypes.Contains(entityType) == false)
                    {
                        continue;
                    }

                    var objectId = entry.Property("Id")?.CurrentValue?.ToString();

                    if (entry.State == EntityState.Added)
                    {
                        var record = new AuditRecord
                        {
                            ObjectId = objectId,
                            ObjectType = entityType,
                            DateTimeStamp = DateTime.UtcNow,
                            OperationType = OperationType.Create,
                            UserName = Thread.CurrentPrincipal?.Identity?.Name
                        };
                        foreach (var prop in entry.CurrentValues.Properties)
                        {
                            var currentValue = entry.CurrentValues[prop];
                            // var property = entityType.GetProperty(prop, flags);

                            var auditDelta = new AuditDelta
                            {
                                FieldName = prop.PropertyInfo.Name,
                                FieldType = prop.PropertyInfo.PropertyType,
                                ValueAfter = currentValue
                            };
                            record.Changes.Add(auditDelta);
                        }
                        auditProcessor.Audit(record);
                    }
                    if (entry.State == EntityState.Modified)
                    {
                        var record = new AuditRecord
                        {
                            ObjectId = objectId,
                            ObjectType = entityType,
                            DateTimeStamp = DateTime.UtcNow,
                            OperationType = OperationType.Update,
                            UserName = Thread.CurrentPrincipal?.Identity?.Name
                        };
                        foreach (var prop in entry.OriginalValues.Properties)
                        {
                            var currentValue = entry.CurrentValues[prop];
                            var originalValue = entry.OriginalValues[prop];

                            //    var property = entityType.GetProperty(prop, flags);

                            var auditDelta = new AuditDelta
                            {
                                FieldName = prop.PropertyInfo.Name,
                                FieldType = prop.PropertyInfo.PropertyType,
                                ValueBefore = originalValue,
                                ValueAfter = currentValue
                            };
                            record.Changes.Add(auditDelta);
                        }
                        auditProcessor.Audit(record);
                    }
                    if (entry.State == EntityState.Deleted)
                    {
                        var record = new AuditRecord
                        {
                            ObjectId = objectId,
                            ObjectType = entityType,
                            DateTimeStamp = DateTime.UtcNow,
                            OperationType = OperationType.Delete,
                            UserName = Thread.CurrentPrincipal?.Identity?.Name
                        };
                        foreach (var prop in entry.OriginalValues.Properties)
                        {
                            var originalValue = entry.OriginalValues[prop];

                            //var property = entityType.GetProperty(prop, flags);

                            var auditDelta = new AuditDelta
                            {
                                FieldName = prop.PropertyInfo.Name,
                                FieldType = prop.PropertyInfo.PropertyType,
                                ValueBefore = originalValue
                            };
                            record.Changes.Add(auditDelta);
                        }
                        auditProcessor.Audit(record);
                    }
                }
            }
        }
    }
}