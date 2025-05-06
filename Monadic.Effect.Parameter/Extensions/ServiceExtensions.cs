using System.Text.Json;
using Monadic.Effect.Configuration.MonadicEffectBuilder;
using Monadic.Effect.Extensions;
using Monadic.Effect.Parameter.Services.ParameterEffectProviderFactory;
using Monadic.Effect.Services.EffectProviderFactory;
using Monadic.Effect.Utils;

namespace Monadic.Effect.Parameter.Extensions;

/// <summary>
/// Provides extension methods for configuring Monadic.Effect.Parameter services in the dependency injection container.
/// </summary>
/// <remarks>
/// The ServiceExtensions class contains utility methods that simplify the registration
/// of Monadic.Effect.Parameter services with the dependency injection system.
///
/// These extensions enable parameter serialization support for the Monadic.Effect system,
/// allowing workflow input and output parameters to be serialized to and from JSON format.
///
/// By using these extensions, applications can easily configure and use the
/// Monadic.Effect.Parameter system with minimal boilerplate code.
/// </remarks>
public static class ServiceExtensions
{
    /// <summary>
    /// Adds parameter serialization support to the Monadic effect configuration builder.
    /// </summary>
    /// <param name="builder">The Monadic effect configuration builder</param>
    /// <param name="jsonSerializerOptions">Optional JSON serializer options to use for parameter serialization</param>
    /// <returns>The configuration builder for method chaining</returns>
    /// <remarks>
    /// This method configures the Monadic.Effect system to serialize workflow input and output
    /// parameters to JSON format. It registers the necessary services with the dependency
    /// injection container and configures the JSON serialization options.
    ///
    /// The method performs the following steps:
    /// 1. Sets the JSON serializer options to use for parameter serialization
    /// 2. Registers the ParameterEffectProviderFactory as an IEffectProviderFactory
    ///
    /// If no JSON serializer options are provided, the default options from MonadicJsonSerializationOptions
    /// are used. These default options are configured to handle common serialization scenarios
    /// in the Monadic.Effect system.
    ///
    /// Example usage:
    /// ```csharp
    /// services.AddMonadicEffects(options =>
    ///     options.SaveWorkflowParameters()
    /// );
    /// ```
    ///
    /// Or with custom JSON serializer options:
    /// ```csharp
    /// var jsonOptions = new JsonSerializerOptions
    /// {
    ///     PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
    ///     WriteIndented = true
    /// };
    ///
    /// services.AddMonadicEffects(options =>
    ///     options.SaveWorkflowParameters(jsonOptions)
    /// );
    /// ```
    /// </remarks>
    public static MonadicEffectConfigurationBuilder SaveWorkflowParameters(
        this MonadicEffectConfigurationBuilder builder,
        JsonSerializerOptions? jsonSerializerOptions = null
    )
    {
        jsonSerializerOptions ??= MonadicJsonSerializationOptions.Default;

        builder.WorkflowParameterJsonSerializerOptions = jsonSerializerOptions;

        return builder.AddEffect<IEffectProviderFactory, ParameterEffectProviderFactory>();
    }
}
