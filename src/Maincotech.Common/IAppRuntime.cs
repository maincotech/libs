using Maincotech.Adapter;
using Maincotech.Logging;
using Maincotech.Reflection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Maincotech
{
    public interface IAppRuntime
    {
        IServiceProvider ServiceProvider { get; }
        ILoggerFactory LoggerFactory { get; }
        ITypeAdapterFactory TypeAdapterFactory { get; }
        ITypeFinder TypeFinder { get; }

        void ConfigureServices(IServiceCollection services, IConfiguration configuration);
    }
}