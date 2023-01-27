using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Maincotech.Common.Licensing
{
    public interface ILicenseVerifyRule
    {
        void Verify(SignedLicense signedLicense);
    }
}
