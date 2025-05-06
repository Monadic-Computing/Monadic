using FluentAssertions;
using LanguageExt;
using Microsoft.Extensions.DependencyInjection;
using Monadic.ArrayLogger.Services.ArrayLoggingProvider;
using Monadic.Effect.Enums;
using Monadic.Effect.Extensions;
using Monadic.Effect.Services.EffectWorkflow;

namespace Monadic.Tests.Effect.Json.Integration.IntegrationTests;

public class JsonEffectProviderTests : TestSetup
{
    public override ServiceProvider ConfigureServices(IServiceCollection services) =>
        services.AddScopedMonadicWorkflow<ITestWorkflow, TestWorkflow>().BuildServiceProvider();

    [Theory]
    public async Task TestJsonEffect()
    {
        // Arrange
        var workflow = Scope.ServiceProvider.GetRequiredService<ITestWorkflow>();
        var workflowTwo = Scope.ServiceProvider.GetRequiredService<ITestWorkflow>();
        var arrayProvider = Scope.ServiceProvider.GetRequiredService<IArrayLoggingProvider>();

        // Act
        await workflow.Run(Unit.Default);
        await workflowTwo.Run(Unit.Default);

        // Assert
        workflow.Metadata.Name.Should().Be("TestWorkflow");
        workflow.Metadata.FailureException.Should().BeNullOrEmpty();
        workflow.Metadata.FailureReason.Should().BeNullOrEmpty();
        workflow.Metadata.FailureStep.Should().BeNullOrEmpty();
        workflow.Metadata.WorkflowState.Should().Be(WorkflowState.Completed);
        arrayProvider.Loggers.Should().NotBeNullOrEmpty();
        arrayProvider.Loggers.Should().HaveCount(2);
    }

    private class TestWorkflow : EffectWorkflow<Unit, Unit>, ITestWorkflow
    {
        protected override async Task<Either<Exception, Unit>> RunInternal(Unit input) =>
            Activate(input).Resolve();
    }

    private interface ITestWorkflow : IEffectWorkflow<Unit, Unit> { }
}
