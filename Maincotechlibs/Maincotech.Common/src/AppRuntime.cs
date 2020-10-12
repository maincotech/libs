using Autofac;
using Autofac.Extensions.DependencyInjection;
using AutoMapper;
using Maincotech.Adapter;
using Maincotech.DependencyInjection;
using Maincotech.Logging;
using Maincotech.Reflection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Reflection;

namespace Maincotech
{
    public class AppRuntime : IAppRuntime
    {
        public IServiceProvider ServiceProvider { get; protected set; }

        public ILoggerFactory LoggerFactory { get; protected set; }

        public ITypeAdapterFactory TypeAdapterFactory { get; protected set; }

        public ITypeFinder TypeFinder { get; protected set; }

        public AppRuntime()
        {
            Initialize();
        }

        protected virtual void InitTypeFinder()
        {
            TypeFinder = new AppDomainTypeFinder();
        }

        protected virtual void Initialize()
        {
            //Init Type finder
            InitTypeFinder();

            //Init log factory
            InitLog();

            //Init TypeAdapter factory
            InitTypeAdapter();

            //Run Startup Tasks
            RunStartupTasks();

            //resolve assemblies here. otherwise, plugins can throw an exception when rendering views
            AppDomain.CurrentDomain.AssemblyResolve += CurrentDomain_AssemblyResolve;
        }

        public virtual void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            //Init Startups
            var startups = TypeFinder.ClassesOfType<IStartup>();
            var instances = startups
             .Select(x => (IStartup)Activator.CreateInstance(x))
             .OrderBy(x => x.Order);

            foreach (var item in instances)
            {
                item.ConfigureServices(services, configuration);
            }

            //Init service provider
            InitServiceProvider(services);
        }

        private Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
        {
            //check for assembly already loaded
            var assembly = AppDomain.CurrentDomain.GetAssemblies().FirstOrDefault(a => a.FullName == args.Name);
            if (assembly != null)
                return assembly;

            //get assembly from TypeFinder
            assembly = TypeFinder?.GetAssemblies().FirstOrDefault(a => a.FullName == args.Name);
            return assembly;
        }

        protected virtual void RunStartupTasks()
        {
            //find startup tasks provided by other assemblies
            var startupTasks = TypeFinder.ClassesOfType<IStartupTask>();

            //create and sort instances of startup tasks
            //we startup this interface even for not installed plugins.
            //otherwise, DbContext initializers won't run and a plugin installation won't work
            var instances = startupTasks
                .Select(startupTask => (IStartupTask)Activator.CreateInstance(startupTask))
                .OrderBy(startupTask => startupTask.Order);

            //execute tasks
            foreach (var task in instances)
            {
                task.Execute();
            }
        }

        protected virtual void InitLog()
        {
            LoggerFactory = new LoggerFactory();
        }

        protected virtual void InitServiceProvider(IServiceCollection services)
        {
            var containerBuilder = new ContainerBuilder();

            //register IAppRuntime
            containerBuilder.RegisterInstance(this).As<IAppRuntime>().SingleInstance();

            //register type finder
            containerBuilder.RegisterInstance(TypeFinder).As<ITypeFinder>().SingleInstance();

            //populate Autofac container builder with the set of registered service descriptors
            containerBuilder.Populate(services);

            //find dependency registrars provided by other assemblies
            var dependencyRegistrars = TypeFinder.ClassesOfType<IDependencyRegistrar>();

            //create and sort instances of dependency registrars
            var instances = dependencyRegistrars
                .Select(dependencyRegistrar => (IDependencyRegistrar)Activator.CreateInstance(dependencyRegistrar))
                .OrderBy(dependencyRegistrar => dependencyRegistrar.Order);

            //register all provided dependencies
            foreach (var dependencyRegistrar in instances)
            {
                dependencyRegistrar.Register(containerBuilder);
            }
            //create service provider
            ServiceProvider = new DependencyInjection.ServiceProvider(containerBuilder.Build());
        }

        protected virtual void InitTypeAdapter()
        {
            //find mapper configurations provided by other assemblies
            var mapperConfigurations = TypeFinder.ClassesOfType<IOrderedMapperProfile>();

            //create and sort instances of mapper configurations
            var instances = mapperConfigurations
                .Select(mapperConfiguration => (IOrderedMapperProfile)Activator.CreateInstance(mapperConfiguration))
                .OrderBy(mapperConfiguration => mapperConfiguration.Order);

            //create AutoMapper configuration
            var config = new MapperConfiguration(cfg =>
            {
                foreach (var instance in instances)
                {
                    cfg.AddProfile(instance.GetType());
                }
            });

            //register
            var typeAdapterFactory = new AutoMappingTypeAdapterFactory();
            typeAdapterFactory.Init(config);

            TypeAdapterFactory = typeAdapterFactory;
        }
    }
}