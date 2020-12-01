using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading;

namespace Maincotech.Domain
{
    public abstract class DomainObject : IEntity
    {
        public static readonly List<string> NotMergedProperties = new List<string>()
        {
            "CreatedBy",
            "CreationTime",
            "LastModifiedTime",
            "ModifiedBy",
            "RowVersion",
            "Id"
        };

        protected DomainObject()
        {
            ToStringIgnoreProperties = new List<string>();
        }

        public string CreatedBy { get; set; }
        public DateTime CreationTime { get; set; }
        public DateTime LastModifiedTime { get; set; }
        public string ModifiedBy { get; set; }

        [Timestamp]
        public byte[] RowVersion { get; set; }

        protected List<string> ToStringIgnoreProperties { get; }

        public Guid Id { get; set; }

        public override string ToString()
        {
            return this.SafeToString(ToStringIgnoreProperties);
        }

        public static TDomainObject ForCreate<TDomainObject>() where TDomainObject : DomainObject, new()
        {
            var domainObject = new TDomainObject
            {
                Id = Guid.NewGuid(),
                CreationTime = DateTime.UtcNow,
                LastModifiedTime = DateTime.UtcNow,
                CreatedBy = AppRuntimeContext.Current.Principal?.Identity?.Name,
                ModifiedBy = AppRuntimeContext.Current.Principal?.Identity?.Name
            };
            return domainObject;
        }

        public static TDomainObject ForUpdate<TDomainObject>(Guid id) where TDomainObject : DomainObject, new()
        {
            var domainObject = new TDomainObject
            {
                Id = id,
                LastModifiedTime = DateTime.UtcNow,
                ModifiedBy = AppRuntimeContext.Current.Principal?.Identity?.Name
            };
            return domainObject;
        }
    }
}