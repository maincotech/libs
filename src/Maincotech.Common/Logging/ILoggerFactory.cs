using System;

namespace Maincotech.Logging
{
    public interface ILoggerFactory
    {
        ILogger CreateLog(Type loggingType);
    }
}