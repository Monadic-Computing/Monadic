using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Monadic.Effect.Configuration.MonadicEffectBuilder;
using Monadic.Effect.Data.Enums;
using Monadic.Effect.Data.Postgres.Services.PostgresContext;
using Monadic.Effect.Data.Postgres.Services.PostgresContextFactory;
using Monadic.Effect.Data.Postgres.Utils;
using Monadic.Effect.Data.Services.DataContext;
using Monadic.Effect.Data.Services.DataContextLoggingProvider;
using Monadic.Effect.Data.Services.IDataContextFactory;
using Monadic.Effect.Extensions;
using Npgsql;

namespace Monadic.Effect.Data.Postgres.Extensions;

/// <summary>
/// Provides extension methods for configuring Monadic.Effect.Data.Postgres services in the dependency injection container.
/// </summary>
/// <remarks>
/// The ServiceExtensions class contains utility methods that simplify the registration
/// of Monadic.Effect.Data.Postgres services with the dependency injection system.
///
/// These extensions enable:
/// 1. Easy configuration of PostgreSQL database contexts
/// 2. Automatic database migration
/// 3. Integration with the Monadic.Effect configuration system
///
/// By using these extensions, applications can easily configure and use the
/// Monadic.Effect.Data.Postgres system with minimal boilerplate code.
/// </remarks>
public static class ServiceExtensions
{
    /// <summary>
    /// Adds PostgreSQL database support to the Monadic.Effect system.
    /// </summary>
    /// <param name="configurationBuilder">The Monadic effect configuration builder</param>
    /// <param name="connectionString">The connection string to the PostgreSQL database</param>
    /// <returns>The configuration builder for method chaining</returns>
    /// <remarks>
    /// This method configures the Monadic.Effect system to use PostgreSQL for workflow metadata persistence.
    /// It performs the following steps:
    ///
    /// 1. Migrates the database schema to the latest version using the DatabaseMigrator
    /// 2. Creates a data source with the necessary enum mappings
    /// 3. Registers a DbContextFactory for creating PostgresContext instances
    /// 4. Enables data context logging
    /// 5. Registers the PostgresContextProviderFactory as an IDataContextProviderFactory
    ///
    /// The PostgreSQL implementation is suitable for production environments where
    /// persistent storage and advanced database features are required.
    ///
    /// Example usage:
    /// ```csharp
    /// services.AddMonadicEffects(options =>
    ///     options.AddPostgresEffect("Host=localhost;Database=Monadic;Username=postgres;Password=password")
    /// );
    /// ```
    /// </remarks>
    public static MonadicEffectConfigurationBuilder AddPostgresEffect(
        this MonadicEffectConfigurationBuilder configurationBuilder,
        string connectionString
    )
    {
        // Migrate the database schema to the latest version
        DatabaseMigrator.Migrate(connectionString).Wait();

        // Create a data source with enum mappings
        var dataSource = ModelBuilderExtensions.BuildDataSource(connectionString);

        // Register the DbContextFactory
        configurationBuilder.ServiceCollection.AddDbContextFactory<PostgresContext>(
            (_, options) =>
            {
                options
                    .UseNpgsql(dataSource)
                    .UseLoggerFactory(new NullLoggerFactory())
                    .ConfigureWarnings(x => x.Log(CoreEventId.ManyServiceProvidersCreatedWarning));
            }
        );

        // Enable data context logging
        configurationBuilder.DataContextLoggingEffectEnabled = true;

        // Register the PostgresContextProviderFactory
        return configurationBuilder.AddEffect<
            IDataContextProviderFactory,
            PostgresContextProviderFactory
        >();
    }
}
