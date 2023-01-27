namespace Maincotech.ModuleModel
{
    public interface IModule : IStartup, IShutdown
    {
        ModuleDescriptor ModuleDescriptor { get; }

        void Uninstall();
    }
}