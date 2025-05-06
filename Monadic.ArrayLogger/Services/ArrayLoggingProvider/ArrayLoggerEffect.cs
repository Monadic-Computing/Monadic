using Microsoft.Extensions.Logging;
using Monadic.Effect.Models.Log;
using Monadic.Effect.Models.Log.DTOs;

namespace Monadic.ArrayLogger.Services.ArrayLoggingProvider;

public class ArrayLoggerEffect(string categoryName) : ILogger
{
    public List<Log> Logs { get; } = [];

    public void Log<TState>(
        LogLevel logLevel,
        EventId eventId,
        TState state,
        Exception? exception,
        Func<TState, Exception?, string> formatter
    )
    {
        var message = formatter(state, exception);

        var log = Effect.Models.Log.Log.Create(
            new CreateLog
            {
                Level = logLevel,
                Message = message,
                CategoryName = categoryName,
                Exception = exception,
                EventId = eventId.Id,
            }
        );

        Logs.Add(log);
    }

    public bool IsEnabled(LogLevel logLevel) => true;

    public IDisposable? BeginScope<TState>(TState state)
        where TState : notnull => null;
}
