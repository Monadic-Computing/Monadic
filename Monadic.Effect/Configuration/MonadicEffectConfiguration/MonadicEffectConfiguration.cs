using System.Text.Json;
using Monadic.Effect.Utils;

namespace Monadic.Effect.Configuration.MonadicEffectConfiguration;

public class MonadicEffectConfiguration : IMonadicEffectConfiguration
{
    public JsonSerializerOptions WorkflowParameterJsonSerializerOptions { get; set; } =
        MonadicJsonSerializationOptions.Default;
}
