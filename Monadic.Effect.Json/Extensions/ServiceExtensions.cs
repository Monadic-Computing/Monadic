using Monadic.Effect.Configuration.MonadicEffectBuilder;
using Monadic.Effect.Extensions;
using Monadic.Effect.Json.Services.JsonEffect;
using Monadic.Effect.Json.Services.JsonEffectFactory;
using Monadic.Effect.Services.EffectProviderFactory;
using Microsoft.Extensions.DependencyInjection;

namespace Monadic.Effect.Json.Extensions;

/// <summary>
/// Provides extension methods for configuring Monadic.Effect.Json services in the dependency injection container.
/// </summary>
/// <remarks>
/// The ServiceExtensions class contains utility methods that simplify the registration
/// of Monadic.Effect.Json services with the dependency injection system.
///
/// These extensions enable JSON serialization support for the Monadic.Effect system,
/// allowing workflow models to be serialized to and from JSON format.
///
/// By using these extensions, applications can easily configure and use the
/// Monadic.Effect.Json system with minimal boilerplate code.
/// </remarks>
public static class ServiceExtensions
{
    /// <summary>
    /// Adds JSON effect support to the Monadic effect configuration builder.
    /// </summary>
    /// <param name="configurationBuilder">The Monadic effect configuration builder</param>
    /// <returns>The configuration builder for method chaining</returns>
    /// <remarks>
    /// This method configures the Monadic.Effect system to use JSON serialization for
    /// tracking and logging model changes. It registers the necessary services with the
    /// dependency injection container.
    ///
    /// The method performs the following steps:
    /// 1. Registers the JsonEffectProvider as a transient service
    /// 2. Registers the JsonEffectProviderFactory as an IEffectProviderFactory
    ///
    /// This enables the Monadic.Effect system to track model changes and serialize them
    /// to JSON format for logging or persistence.
    ///
    /// Example usage:
    /// ```csharp
    /// services.AddMonadicEffects(options =>
    ///     options.AddJsonEffect()
    /// );
    /// ```
    /// </remarks>
    public static MonadicEffectConfigurationBuilder AddJsonEffect(
        this MonadicEffectConfigurationBuilder configurationBuilder
    )
    {
        configurationBuilder.ServiceCollection.AddTransient<
            IJsonEffectProvider,
            JsonEffectProvider
        >();

        return configurationBuilder.AddEffect<IEffectProviderFactory, JsonEffectProviderFactory>();
    }
}
