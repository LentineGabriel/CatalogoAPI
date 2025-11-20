using System.Collections.Concurrent;
using System.Xml.Linq;

namespace CatagoloAPI.Logging;
public class CustomLoggerProvider : ILoggerProvider
{
    #region Props/Ctor
    readonly CustomLoggerProviderConfig loggerConfig;
    readonly ConcurrentDictionary<string , CustomerLogger> loggers = new ConcurrentDictionary<string , CustomerLogger>();

    public CustomLoggerProvider(CustomLoggerProviderConfig config)
    {
        loggerConfig = config;
    }
    #endregion

    #region Methods
    public ILogger CreateLogger(string categoryName) => loggers.GetOrAdd(categoryName, name => new CustomerLogger(name , loggerConfig));

    public void Dispose() => loggers.Clear();
    #endregion
}
