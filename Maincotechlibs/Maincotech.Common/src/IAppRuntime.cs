using Maincotech.Adapter;
using Maincotech.Logging;
using Maincotech.Reflection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Security.Principal;

namespace Maincotech
{
    public interface IAppRuntime
    {
        IServiceProvider ServiceProvider { get; }
        ILoggerFactory LoggerFactory { get; }
        ITypeAdapterFactory TypeAdapterFactory { get; }
        ITypeFinder TypeFinder { get; }
        IPrincipal Principal { get; }

        void ConfigureServices(IServiceCollection services, IConfiguration configuration);
    }
}