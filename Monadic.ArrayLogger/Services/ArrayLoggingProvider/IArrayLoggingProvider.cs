using Microsoft.Extensions.Logging;

namespace Monadic.ArrayLogger.Services.ArrayLoggingProvider;

public interface IArrayLoggingProvider : ILoggerProvider
{
    public List<ArrayLoggerEffect> Loggers { get; }
}
