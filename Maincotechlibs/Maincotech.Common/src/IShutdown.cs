using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Maincotech
{
    public interface IShutdown
    {
        /// <summary>
        /// Remove any of the middleware
        /// </summary>
        /// <param name="services">Collection of service descriptors</param>
        /// <param name="configuration">Configuration of the application</param>
        void UnConfigureServices(IServiceCollection services, IConfiguration configuration);
    }
}