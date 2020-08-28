using System.Collections.Generic;

namespace Maincotech.Services
{
    public interface IHostService
    {
        HealthState CheckHealth();

        IEnumerable<HostServer> GetHostServers();

       // Response ProcessRequest(IRequest request);
    }
}