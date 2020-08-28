using Serilog;
using System;
using System.IO;

namespace Maincotech.Logging
{
    /// <summary>
    /// Default logger factory
    /// </summary>
    public class LoggerFactory : ILoggerFactory
    {
        public LoggerFactory()
        {
            if (Directory.Exists(AppRuntimeContext.LogsFolderPath))
            {
                Directory.CreateDirectory(AppRuntimeContext.LogsFolderPath);
            }

            Log.Logger = new LoggerConfiguration()
                .WriteTo.File(Path.Combine(AppRuntimeContext.LogsFolderPath, "log-.txt"), rollingInterval: RollingInterval.Day)
                .MinimumLevel.Information()
                .CreateLogger();
        }

        public ILogger CreateLog(Type loggingType)
        {
            return CreateLogger(loggingType);
        }

        public static ILogger CreateLogger(Type loggingType)
        {
            return new SerilogLogger(loggingType);
        }
    }
}