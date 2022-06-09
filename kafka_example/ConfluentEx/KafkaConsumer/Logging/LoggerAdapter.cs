using Microsoft.Extensions.Logging;

public class LoggerAdapter<T>:ILoggerAdapter<T>
{
    private readonly ILogger<T> _logger;

    public LoggerAdapter(ILogger<T> logger)
    {
        _logger = logger;
    }

    public void LogInformation(string? message, params string[][] args)
    {
        _logger.LogInformation(message, args);
    }
}