
namespace CatagoloAPI.Logging;
public class CustomerLogger : ILogger
{
    readonly string loggerName;
    readonly CustomLoggerProviderConfig loggerConfig;

    public CustomerLogger(string name , CustomLoggerProviderConfig config)
    {
        loggerName = name;
        loggerConfig = config;
    }

    public IDisposable? BeginScope<TState>(TState state)
    {
        return null;
    }

    public bool IsEnabled(LogLevel logLevel)
    {
        return logLevel == loggerConfig.LogLevel;
    }

    public void Log<TState>(LogLevel logLevel , EventId eventId , TState state , Exception? exception , Func<TState , Exception? , string> formatter)
    {
        string message = $"{logLevel.ToString()}: {eventId.Id} - {formatter(state , exception)}";

        EscreverNoTexto(message);
    }

    private void EscreverNoTexto(string message)
    {
        var caminhoArquivo = Path.Combine(Path.GetTempPath() , "catalogo.log");

        using(StreamWriter sw = new StreamWriter(caminhoArquivo , true))
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
}
