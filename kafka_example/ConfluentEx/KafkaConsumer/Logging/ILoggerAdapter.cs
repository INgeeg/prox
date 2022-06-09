public interface ILoggerAdapter<T>
{
    void LogInformation(string? message, params string[][] args);
}