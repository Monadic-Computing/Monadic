using LanguageExt;
using Monadic.Effect.Services.EffectWorkflow;

namespace Monadic.Tests.Effect.Integration.Examples.Workflows;

public class TestEffectWorkflow
    : EffectWorkflow<TestEffectWorkflowInput, TestEffectWorkflow>,
        ITestEffectWorkflow
{
    protected override async Task<Either<Exception, TestEffectWorkflow>> RunInternal(
        TestEffectWorkflowInput input
    ) => Activate(input, this).Resolve();
}

public record TestEffectWorkflowInput();

public interface ITestEffectWorkflow
    : IEffectWorkflow<TestEffectWorkflowInput, TestEffectWorkflow> { }
