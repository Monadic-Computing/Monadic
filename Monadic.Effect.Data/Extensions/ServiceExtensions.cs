using System.Collections.Concurrent;
using Monadic.Effect.Configuration.MonadicEffectBuilder;
using Monadic.Effect.Data.Enums;
using Monadic.Effect.Data.Services.DataContextLoggingProvider;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Monadic.Effect.Data.Extensions;

/// <summary>
/// Provides extension methods for configuring Monadic.Effect.Data services in the dependency injection container.
/// </summary>
/// <remarks>
/// The ServiceExtensions class contains utility methods that simplify the registration
/// of Monadic.Effect.Data services with the dependency injection system.
///
/// These extensions enable:
/// 1. Easy configuration of data context logging
/// 2. Consistent service registration across different applications
/// 3. Integration with the Monadic.Effect configuration system
///
/// By using these extensions, applications can easily configure and use the
/// Monadic.Effect.Data system with minimal boilerplate code.
/// </remarks>
public static class ServiceExtensions
{
    /// <summary>
    /// Adds data context logging to the Monadic.Effect system.
    /// </summary>
    /// <param name="configurationBuilder">The Monadic effect configuration builder</param>
    /// <param name="minimumLogLevel">The minimum log level to capture (defaults to Information if not specified)</param>
    /// <param name="blacklist">A list of namespace patterns to exclude from logging</param>
    /// <returns>The configuration builder for method chaining</returns>
    /// <exception cref="Exception">Thrown if data context logging is not enabled</exception>
    /// <remarks>
    /// This method configures logging for database operations in the Monadic.Effect.Data system.
    /// It registers the necessary services for capturing and processing database logs.
    ///
    /// The method:
    /// 1. Checks for a log level specified in the CHAIN_SHARP_POSTGRES_LOG_LEVEL environment variable
    /// 2. Verifies that data context logging is enabled in the configuration
    /// 3. Creates a logging configuration with the specified settings
    /// 4. Registers the logging provider and configuration with the dependency injection container
    ///
    /// Data context logging provides visibility into:
    /// - SQL queries executed
    /// - Transaction boundaries
    /// - Errors and warnings
    ///
    /// This is particularly useful for debugging and performance optimization.
    ///
    /// Example usage:
    /// ```csharp
    /// services.AddMonadicEffects(options =>
    ///     options
    ///         .AddPostgresEffect(connectionString)
    ///         .AddEffectDataContextLogging(
    ///             minimumLogLevel: LogLevel.Information,
    ///             blacklist: ["Microsoft.EntityFrameworkCore.*"]
    ///         )
    /// );
    /// ```
    /// </remarks>
    public static MonadicEffectConfigurationBuilder AddEffectDataContextLogging(
        this MonadicEffectConfigurationBuilder configurationBuilder,
        LogLevel? minimumLogLevel = null,
        List<string>? blacklist = null
    )
    {
        // Check for log level in environment variables
        var logLevelEnvironment = Environment.GetEnvironmentVariable(
            "CHAIN_SHARP_POSTGRES_LOG_LEVEL"
        );

        var parsed = Enum.TryParse<LogLevel>(logLevelEnvironment, out var logLevel);

        if (parsed)
            minimumLogLevel ??= logLevel;

        // Verify that data context logging is enabled
        if (configurationBuilder.DataContextLoggingEffectEnabled == false)
            throw new Exception(
                "Data Context Logging effect is not enabled in Monadic. Ensure a Data Effect has been added to MonadicEffects (before calling AddEffectDataContextLogging). e.g. .AddMonadicEffects(x => x.AddPostgresEffect(connectionString).AddDataContextEffectLogging())"
            );

        // Create and register the logging configuration
        var credentials = new DataContextLoggingProviderConfiguration
        {
            MinimumLogLevel = minimumLogLevel ?? LogLevel.Information,
            Blacklist = blacklist ?? []
        };

        configurationBuilder
            .ServiceCollection.AddSingleton<IDataContextLoggingProviderConfiguration>(credentials)
            .AddSingleton<ILoggerProvider, DataContextLoggingProvider>();

        return configurationBuilder;
    }
}
