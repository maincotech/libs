using Autofac;

namespace Maincotech.DependencyInjection
{
    /// <summary>
    /// Dependency registrar interface
    /// </summary>
    public interface IDependencyRegistrar
    {
        int Order { get; }

        void Register(ContainerBuilder builder);
    }
}