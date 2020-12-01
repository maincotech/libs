using System;

namespace Maincotech.Domain
{
    public interface IMayHaveTenant
    {
        public Guid? TenantId { get; set; }
    }
}