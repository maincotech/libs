using System;

namespace Maincotech.Logging
{
    public class SerilogLogger : ILogger
    {
        public Type Source { get; }
        private Serilog.ILogger _InnerLogger;

        public SerilogLogger(Type loggingType)
        {
            Source = loggingType;
            _InnerLogger = Serilog.Log.ForContext(loggingType);
        }

        public void Debug(string message, params object[] args)
        {
            _InnerLogger.Debug(message, args);
        }

        public void Debug(string message, Exception exception, params object[] args)
        {
            _InnerLogger.Debug(exception, message, args);
        }

        public void Error(string message, params object[] args)
        {
            _InnerLogger.Error(message, args);
        }

        public void Error(string message, Exception exception, params object[] args)
        {
            _InnerLogger.Error(exception, message, args);
        }

        public void Fatal(string message, params object[] args)
        {
            _InnerLogger.Error(message, args);
        }

        public void Fatal(string message, Exception exception, params object[] args)
        {
            _InnerLogger.Error(exception, message, args);
        }

        public void Info(string message, params object[] args)
        {
            _InnerLogger.Information(message, args);
        }

        public void Warning(string message, params object[] args)
        {
            _InnerLogger.Warning(message, args);
        }
    }
}