using System;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using Maincotech.Utilities;

namespace Maincotech
{
    public class AppRuntimeContext
    {
        private readonly static string _executingPath;

        /// <summary>
        /// Current executing path
        /// </summary>
        public static string ExecutingPath
        {
            get { return _executingPath; }
        }

        public static string ModuleFolderPath => Path.Combine(ExecutingPath, "Modules");
        public static string LogsFolderPath => Path.Combine(ExecutingPath, "Logs");

        static AppRuntimeContext()
        {

            string path;
            try
            {
                path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            }
            catch (NotSupportedException)
            {
                path = Directory.GetCurrentDirectory();
            }
            _executingPath = path;
            if (OSUtil.IsMacOS())
            {
                _executingPath = path.Replace("file:",string.Empty);
            }
            if(OSUtil.IsWindows())
            {
                _executingPath = new Uri(path).LocalPath;
            }
        }

        #region Methods

        [MethodImpl(MethodImplOptions.Synchronized)]
        public static IAppRuntime Create()
        {
            //create NopEngine as engine
            return Singleton<IAppRuntime>.Instance ?? (Singleton<IAppRuntime>.Instance = new AppRuntime());
        }

        public static void Replace(IAppRuntime appRuntime)
        {
            Singleton<IAppRuntime>.Instance = appRuntime;
        }

        #endregion Methods

        #region Properties

        public static IAppRuntime Current
        {
            get
            {
                if (Singleton<IAppRuntime>.Instance == null)
                {
                    Create();
                }

                return Singleton<IAppRuntime>.Instance;
            }
        }

        #endregion Properties
    }
}