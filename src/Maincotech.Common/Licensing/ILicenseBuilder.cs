using Maincotech.Common.Security.Signing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Maincotech.Common.Licensing
{
    public interface ILicenseBuilder
    {
        ILicenseBuilder WithSigner(ISigner signer);
        ILicenseBuilder WithSerialNumber(string serialNumber);
        ILicenseBuilder ExpiresOn(DateTime dateTime);
        ILicenseBuilder WithProperty(string key, string value);
        SignedLicense Build();
    }
}
