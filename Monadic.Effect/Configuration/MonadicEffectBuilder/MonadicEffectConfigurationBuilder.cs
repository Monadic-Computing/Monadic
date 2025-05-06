using System.Text.Json;
using System.Text.Json.Serialization;
using Monadic.Effect.Utils;
using Microsoft.Extensions.DependencyInjection;

namespace Monadic.Effect.Configuration.MonadicEffectBuilder;

public class MonadicEffectConfigurationBuilder(IServiceCollection serviceCollection)
{
    public IServiceCollection ServiceCollection => serviceCollection;

    public bool DataContextLoggingEffectEnabled { get; set; } = false;

    public JsonSerializerOptions WorkflowParameterJsonSerializerOptions { get; set; } =
        MonadicJsonSerializationOptions.Default;

    protected internal MonadicEffectConfiguration.MonadicEffectConfiguration Build() =>
        new() { WorkflowParameterJsonSerializerOptions = WorkflowParameterJsonSerializerOptions };
}
