using System.Text.Json;

namespace Monadic.Effect.Configuration.MonadicEffectConfiguration;

public interface IMonadicEffectConfiguration
{
    public JsonSerializerOptions WorkflowParameterJsonSerializerOptions { get; }
}
