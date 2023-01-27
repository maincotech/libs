using Maincotech.Configuration;
using Maincotech.Logging;
using Maincotech.Reflection;
using Maincotech.Resources;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Primitives;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;

namespace Maincotech.ModuleModel
{
    public class FileBasedModuleManager : IFileBasedModuleManager
    {
        private static readonly ILogger _Logger = AppRuntimeContext.Current.GetLogger<FileBasedModuleManager>();
        private readonly string _ConfigurationFilePath;

        public FileBasedModuleManager()
        {
            _ConfigurationFilePath = Path.Combine(AppRuntimeContext.ExecutingPath, "modulesettings.json");
        }

        private bool _IsLoaded;

        private ModuleConfiguration _ModuleConfiguration;

        public ModuleConfiguration ModuleConfiguration
        {
            get
            {
                if (_IsLoaded)
                {
                    return _ModuleConfiguration;
                }
                LoadModuleConfiguration();
                return _ModuleConfiguration;
            }
        }

        private IServiceCollection _Services;
        private IConfiguration _Configuration;

        public int Order => 1;

        private void LoadModuleConfiguration()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(AppContext.BaseDirectory)
               .AddJsonFile("modulesettings.json", optional: true, reloadOnChange: true);
            var configuration = builder.Build();

            ChangeToken.OnChange(() => configuration.GetReloadToken(), () => { _ModuleConfiguration = configuration.GetSection("Modules").Get<ModuleConfiguration>(); });
            _ModuleConfiguration = configuration.GetSection("Modules").Get<ModuleConfiguration>();
            if (_ModuleConfiguration == null)
            {
                _ModuleConfiguration = new ModuleConfiguration();
            }
            _IsLoaded = true;
        }

        private void SaveModules()
        {
            string output = JsonConvert.SerializeObject(new { Modules = ModuleConfiguration }, Formatting.Indented);
            File.WriteAllText(_ConfigurationFilePath, output);
        }

        //  [MethodImpl(MethodImplOptions.NoInlining)]
        private void UnloadModule(string moduleIdentity, out WeakReference reference, bool isUninstall = false)
        {
            var loadContext = SingletonDictionary<string, ModuleLoadContext>.Instance[moduleIdentity];
            var assembly = loadContext.GetModuleAssembly();
            var moduleTypes = assembly.ClassesOf<IModule>();
            foreach (var moduleType in moduleTypes)
            {
                var module = Activator.CreateInstance(moduleType) as IModule;
                module?.UnConfigureServices(_Services, _Configuration);
                if (isUninstall)
                {
                    module.Uninstall();
                }
            }

            reference = new WeakReference(loadContext);
            loadContext.Unload();
        }

        public OperationResult Disable(string moduleIdentity)
        {
            var result = new OperationResult();
            try
            {
                var moduleInfo = ModuleConfiguration.FirstOrDefault(m => m.Identity == moduleIdentity);
                if (moduleInfo == null)
                {
                    throw new FileNotFoundException(ResourceHelper.GetCommonString("ModuleManager_ModuleNotFoundInConfig", moduleIdentity));
                }
                moduleInfo.Enabled = false;
                SaveModules();
                if (SingletonDictionary<string, ModuleLoadContext>.Instance.ContainsKey(moduleIdentity) == false)
                {
                    throw new FileNotFoundException(ResourceHelper.GetCommonString("ModuleManager_ModuleNotFoundInRuntime", moduleIdentity));
                }

                UnloadModule(moduleIdentity, out WeakReference reference);
                SingletonDictionary<string, ModuleLoadContext>.Instance.Remove(moduleIdentity);

                result.Succeed = true;
                result.StausMessage = ResourceHelper.GetCommonString("ModuleManager_Disable_Succeeded", moduleIdentity);
            }
            catch (Exception ex)
            {
                result.Succeed = false;
                result.ErrorMessage = ex.Message;
                _Logger.Error(ResourceHelper.GetCommonString("ModuleManager_Disable_Failed", moduleIdentity), ex);
            }

            return result;
        }

        public OperationResult Enable(string moduleIdentity)
        {
            var result = new OperationResult();
            try
            {
                var moduleInfo = ModuleConfiguration.FirstOrDefault(m => m.Identity == moduleIdentity);
                if (moduleInfo == null)
                {
                    throw new FileNotFoundException(ResourceHelper.GetCommonString("ModuleManager_ModuleNotFoundInConfig", moduleIdentity));
                }
                var modulePath = Path.Combine(AppRuntimeContext.ModuleFolderPath, moduleInfo.Identity, moduleInfo.File);
                var loadContext = new ModuleLoadContext(modulePath);
                if (SingletonDictionary<string, ModuleLoadContext>.Instance.ContainsKey(moduleInfo.Identity) == false)
                {
                    SingletonDictionary<string, ModuleLoadContext>.Instance.Add(moduleInfo.Identity, loadContext);
                }

                var assembly = loadContext.GetModuleAssembly();
                var moduleTypes = assembly.ClassesOf<IModule>();
                foreach (var moduleType in moduleTypes)
                {
                    var module = Activator.CreateInstance(moduleType) as IModule;
                    module?.ConfigureServices(_Services, _Configuration);
                }

                //save to configuration file
                moduleInfo.Enabled = true;
                SaveModules();
                result.Succeed = true;
                result.StausMessage = ResourceHelper.GetCommonString("ModuleManager_Enable_Succeeded", moduleIdentity);
            }
            catch (Exception ex)
            {
                result.Succeed = false;
                result.ErrorMessage = ex.Message;
                _Logger.Error(ResourceHelper.GetCommonString("ModuleManager_Enable_Failed", moduleIdentity), ex);
            }

            return result;
        }

        public OperationResult Install(ModuleInfo moduleInfo)
        {
            var reuslt = new OperationResult();
            try
            {
                //var modulePath = Path.Combine(AppRuntimeContext.ExecutingPath, "Modules", moduleInfo.Name, moduleInfo.File);
                //AssemblyName assemblyName = new AssemblyName(Path.GetFileNameWithoutExtension(modulePath));
                //var loadContext = new ModuleLoadContext(modulePath);
                //SingletonDictionary<string,ModuleLoadContext>.Instance.Add(moduleInfo.Identity, loadContext);
                //WeakReference<ModuleLoadContext> reference = new WeakReference<ModuleLoadContext>(loadContext, true);
                //var assembly = loadContext.LoadFromAssemblyName(assemblyName);
                //var modules = assembly.ExportedTypes.OfType<IModule>();
                //foreach (var m in modules)
                //{
                //    var module = Activator.CreateInstance(m.GetType()) as IModule;
                //}
                //Save it to configuratoin file
                if (!ModuleConfiguration.Exists(m => m.Identity == moduleInfo.Identity))
                {
                    ModuleConfiguration.Add(moduleInfo);
                    ConfigurationHelper.AddOrUpdateAppSetting("Modules", ModuleConfiguration, _ConfigurationFilePath);
                }
            }
            catch (Exception ex)
            {
                reuslt.Succeed = false;
                reuslt.ErrorMessage = ex.Message;
                _Logger.Error(ResourceHelper.GetCommonString("ModuleManager_Install_Failed", moduleInfo.Name), ex);
            }
            //try
            //{
            //    SingletonDictionary<string, IModule>.Instance.Add(module.Identity, module);
            //    if (!modules.Exists(m => m.Identity == module.Identity))
            //    {
            //        var moduleInfo = module.To<ModuleInfo>();
            //        moduleInfo.Enabled = false;
            //        modules.Add(moduleInfo);
            //        ConfigurationHelper.AddOrUpdateAppSetting("Modules", modules, _ConfigurationFilePath);
            //    }
            //}
            //catch (Exception ex)
            //{
            //    reuslt.Succeed = false;
            //    reuslt.ErrorMessage = ex.Message;
            //    _Logger.Error(ResourceHelper.GetCommonString("ModuleManager_Install_Failed", module.Name), ex);
            //}

            return reuslt;
        }

        public OperationResult Uninstall(string moduleIdentity)
        {
            var result = new OperationResult();
            try
            {
                var moduleInfo = ModuleConfiguration.FirstOrDefault(m => m.Identity == moduleIdentity);
                string moduleFolder = Path.Combine(AppRuntimeContext.ModuleFolderPath, moduleIdentity);
                if (moduleInfo != null)
                {
                    ModuleConfiguration.Remove(moduleInfo);
                    SaveModules();
                }

                if (SingletonDictionary<string, ModuleLoadContext>.Instance.ContainsKey(moduleIdentity))
                {
                    WeakReference reference;
                    UnloadModule(moduleIdentity, out reference, true);
                    SingletonDictionary<string, ModuleLoadContext>.Instance.Remove(moduleIdentity);

                    for (int i = 0; reference.IsAlive && (i < 10); i++)
                    {
                        GC.Collect();
                        GC.WaitForPendingFinalizers();
                    }
                }

                if (!string.IsNullOrEmpty(moduleFolder))
                {
                    Directory.Delete(moduleFolder, true);
                }

                result.Succeed = true;
                result.StausMessage = ResourceHelper.GetCommonString("ModuleManager_Uninstall_Succeeded", moduleIdentity);
            }
            catch (Exception ex)
            {
                result.Succeed = false;
                result.ErrorMessage = ex.Message;
                _Logger.Error(ResourceHelper.GetCommonString("ModuleManager_Uninstall_Failed", moduleIdentity), ex);
            }

            return result;
            //try
            //{
            //    if (SingletonDictionary<string, IModule>.Instance.ContainsKey(moduleIdentity))
            //    {
            //        //Remove it from runtime
            //        var module = SingletonDictionary<string, IModule>.Instance[moduleIdentity];
            //        module.UnConfigureServices(_Services, _Configuration);

            //        SingletonDictionary<string, IModule>.Instance.Remove(moduleIdentity);
            //    }
            //}
            //catch (Exception ex)
            //{
            //    reuslt.ErrorMessage = ex.Message;
            //    _Logger.Error(ResourceHelper.GetCommonString("ModuleManager_Uninstall_Failed", moduleIdentity), ex);
            //}
            //finally
            //{
            //    try
            //    {
            //        var moduleInfo = modules.FirstOrDefault(m => m.Identity == moduleIdentity);
            //        if (moduleInfo != null)
            //        {
            //            modules.Remove(moduleInfo);
            //            ConfigurationHelper.AddOrUpdateAppSetting("Modules", modules, _ConfigurationFilePath);
            //        }

            //        reuslt.Succeed = true;
            //        reuslt.StausMessage = ResourceHelper.GetCommonString("ModuleManager_Uninstall_Succeeded", moduleIdentity);
            //    }
            //    catch (Exception e)
            //    {
            //        reuslt.ErrorMessage += e.Message;
            //        _Logger.Error($"Failed to install module {moduleIdentity}", e);
            //    }
            //}

            //return reuslt;
        }

        public void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            _Services = services;
            _Configuration = configuration;
        }

        public IEnumerable<ModuleInfo> GetModules()
        {
            return ModuleConfiguration.ToList();
        }

        //  [MethodImpl(MethodImplOptions.NoInlining)]
        private bool ValidateModuleAssembly(string assemblyFile, out WeakReference reference)
        {
            //Check IModule interface.
            var moduleLoadContext = new ModuleLoadContext(assemblyFile);
            reference = new WeakReference(moduleLoadContext);
            var assembly = moduleLoadContext.GetModuleAssembly();
            var count = assembly.ClassesOf<IModule>().Count();
            moduleLoadContext.Unload();
            return count == 1;
        }

        public OperationResult Install(ZipArchive moduleZipArchive)
        {
            var result = new OperationResult();
            var firstEntry = moduleZipArchive.Entries.FirstOrDefault();
            var moduleIdentity = firstEntry?.FullName?.TrimEnd('/');
            if (moduleIdentity.IsNullOrEmpty() || firstEntry.Name.IsNotNullOrEmpty())
            {
                result.Succeed = false;
                result.ErrorMessage = "The module file is invalid.";
                return result;
            }
            var moduleFolder = Path.Combine(AppRuntimeContext.ModuleFolderPath, moduleIdentity);
            try
            {
                moduleZipArchive.ExtractToDirectory(AppRuntimeContext.ModuleFolderPath, true);
                var moduleConfigFile = Path.Combine(moduleFolder, "module.json");
                if (File.Exists(moduleConfigFile) == false)
                {
                    throw new FileNotFoundException("The module configuration file is missing. Please check if the module is valid.");
                }
                string json = File.ReadAllText(moduleConfigFile);
                var moduleDescriptor = Newtonsoft.Json.JsonConvert.DeserializeObject<ModuleDescriptor>(json);

                //Check assembly
                var assemblyFile = Path.Combine(moduleFolder, moduleDescriptor.File);
                if (File.Exists(moduleConfigFile) == false)
                {
                    throw new FileNotFoundException("The module assembly file is missing. Please check if the module is valid.");
                }

                WeakReference reference;
                var isValid = ValidateModuleAssembly(assemblyFile, out reference);

                for (int i = 0; reference.IsAlive && (i < 10); i++)
                {
                    GC.Collect();
                    GC.WaitForPendingFinalizers();
                }

                if (isValid == false)
                {
                    throw new InvalidDataException("The module assembly does not implement IModule interface correctly.");
                }

                //Add to config file
                var moduleInfo = moduleDescriptor.To<ModuleInfo>();
                if (!ModuleConfiguration.Exists(m => m.Identity == moduleInfo.Identity))
                {
                    ModuleConfiguration.Add(moduleInfo);
                    SaveModules();
                }

                result.Succeed = true;
                result.StausMessage = ResourceHelper.GetCommonString("ModuleManager_Install_Succeeded", moduleIdentity);
            }
            catch (Exception ex)
            {
                try
                {
                    if (Directory.Exists(moduleFolder))
                    {
                        Directory.Delete(moduleFolder, true);
                    }
                }
                catch (Exception)
                {
                }

                result.Succeed = false;
                result.ErrorMessage = ex.Message;
                _Logger.Error(ResourceHelper.GetCommonString("ModuleManager_Install_Failed", moduleIdentity), ex);
            }

            return result;
        }

        public void LoadModules()
        {
            //Get Embed modules
            var moduleTypes = AppRuntimeContext.Current.TypeFinder.ClassesOfType<FileBasedModule>();
            if (moduleTypes.Any())
            {
                foreach (var moduleType in moduleTypes)
                {
                    var module = Activator.CreateInstance(moduleType) as IModule;
                    module?.ConfigureServices(_Services, _Configuration);
                }
            }

            var enabledModules = ModuleConfiguration.Where(m => m.Enabled);
            if (enabledModules.Any() == false)
            {
                return;
            }
            foreach (var moduleInfo in enabledModules)
            {
                Enable(moduleInfo.Identity);
            }
        }
    }
}