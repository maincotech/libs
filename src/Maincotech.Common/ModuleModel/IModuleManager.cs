using System.Collections.Generic;
using System.IO.Compression;

namespace Maincotech.ModuleModel
{
    public interface IModuleManager<TModule> : IStartup where TModule : class, IModule
    {
        void LoadModules();

        OperationResult Install(ZipArchive moduleZipArchive);

        OperationResult Uninstall(string moduleIdentity);

        OperationResult Disable(string moduleIdentity);

        OperationResult Enable(string moduleIdentity);

        IEnumerable<ModuleInfo> GetModules();
    }
}