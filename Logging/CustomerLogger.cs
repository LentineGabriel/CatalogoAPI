
namespace CatagoloAPI.Logging;
public class CustomerLogger : ILogger
{
    #region Props/Ctor
    readonly string loggerName;
    readonly CustomLoggerProviderConfig loggerConfig;

    public CustomerLogger(string name , CustomLoggerProviderConfig config)
    {
        loggerName = name;
        loggerConfig = config;
    }
    #endregion

    #region Methods
    public IDisposable? BeginScope<TState>(TState state) => null;

    public bool IsEnabled(LogLevel logLevel) => logLevel == loggerConfig.LogLevel;

    public void Log<TState>(LogLevel logLevel , EventId eventId , TState state , Exception? exception , Func<TState , Exception? , string> formatter)
    {
        string message = $"{logLevel.ToString()}: {eventId.Id} - {formatter(state , exception)}";

        WritingInText(message);
    }

    private void WritingInText(string message)
    {
        var path = Path.Combine(Path.GetTempPath() , "catalogo.log");

        using(StreamWriter sw = new StreamWriter(path , true))
        {
            try
            {
                sw.WriteLine(message);
                sw.Close();
            }
            catch(Exception ex)
            {
                Console.WriteLine($"Erro ao escrever no arquivo: {ex.Message}");
            }
        }
    }
    #endregion
}
