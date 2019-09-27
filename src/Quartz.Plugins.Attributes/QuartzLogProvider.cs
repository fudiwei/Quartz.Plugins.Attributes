using System;
using Microsoft.Extensions.Logging;
using Quartz.Logging;

namespace Quartz.Plugins.Attributes
{
    internal class QuartzLogProvider : ILogProvider
    {
        private readonly ILoggerFactory LoggerFactory;

        public QuartzLogProvider(ILoggerFactory loggerFactory)
        {
            LoggerFactory = loggerFactory;
        }

        public Logger GetLogger(string name)
        {
            return (level, func, exception, parameters) =>
            {
                ILogger logger = LoggerFactory?.CreateLogger(name);

                if (logger == null || func == null)
                    return false;

                if (level == Logging.LogLevel.Trace)
                {
                    logger.LogTrace(exception, func.Invoke(), parameters);
                    return true;
                }
                else if (level == Logging.LogLevel.Debug)
                {
                    logger.LogDebug(exception, func.Invoke(), parameters);
                    return true;
                }
                else if (level == Logging.LogLevel.Info)
                {
                    logger.LogInformation(exception, func.Invoke(), parameters);
                    return true;
                }
                else if (level == Logging.LogLevel.Warn)
                {
                    logger.LogWarning(exception, func.Invoke(), parameters);
                    return true;
                }
                else if (level == Logging.LogLevel.Error)
                {
                    logger.LogError(exception, func.Invoke(), parameters);
                    return true;
                }
                else if (level == Logging.LogLevel.Fatal)
                {
                    logger.LogCritical(exception, func.Invoke(), parameters);
                    return true;
                }

                return false;
            };
        }

        public IDisposable OpenMappedContext(string key, string value)
        {
            throw new NotImplementedException();
        }

        public IDisposable OpenNestedContext(string message)
        {
            throw new NotImplementedException();
        }
    }
}
