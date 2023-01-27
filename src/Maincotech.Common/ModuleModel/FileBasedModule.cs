using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Maincotech.ModuleModel
{
    public abstract class FileBasedModule : IModule
    {
        public abstract ModuleDescriptor ModuleDescriptor { get; }

        public abstract int Order { get; }

        public virtual void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
        }

        public virtual void UnConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
        }

        public virtual void Uninstall()
        {
        }
    }
}