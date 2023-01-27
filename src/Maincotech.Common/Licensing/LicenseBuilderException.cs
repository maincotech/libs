using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Maincotech.Common.Licensing
{
    public class LicenseBuilderException:Exception
    {
        public LicenseBuilderException(string message)
            : base(message)
        { }
    }
}
