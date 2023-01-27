using System;
using System.IO;
using System.Reflection;
using System.Runtime.Loader;

namespace Maincotech.ModuleModel
{
    public class ModuleLoadContext : AssemblyLoadContext
    {
        private AssemblyDependencyResolver _resolver;
        private AssemblyName _AssemblyName;

        public string Location { get; private set; }

        public ModuleLoadContext(string location) : base(isCollectible: true)
        {
            Location = location;
            _resolver = new AssemblyDependencyResolver(location);
            _AssemblyName = new AssemblyName(Path.GetFileNameWithoutExtension(location));
        }

        public Assembly GetModuleAssembly()
        {
            return LoadFromAssemblyName(_AssemblyName);
        }

        protected override Assembly Load(AssemblyName assemblyName)
        {
            string assemblyPath = _resolver.ResolveAssemblyToPath(assemblyName);
            if (assemblyPath != null)
            {
                using (var stream = File.OpenRead(assemblyPath))
                {
                    var assembly = LoadFromStream(stream);
                    return assembly;
                }
            }

            return null;
        }

        protected override IntPtr LoadUnmanagedDll(string unmanagedDllName)
        {
            string libraryPath = _resolver.ResolveUnmanagedDllToPath(unmanagedDllName);

            if (libraryPath != null)
            {
                return LoadUnmanagedDllFromPath(libraryPath);
            }

            return IntPtr.Zero;
        }
    }
}