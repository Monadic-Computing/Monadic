using FluentAssertions;
using LanguageExt;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Monadic.Effect.Data.Services.DataContext;
using Monadic.Effect.Data.Services.IDataContextFactory;
using Monadic.Effect.Enums;
using Monadic.Effect.Extensions;
using Monadic.Effect.Models.Metadata.DTOs;
using Monadic.Effect.Services.EffectWorkflow;
using Metadata = Monadic.Effect.Models.Metadata.Metadata;

namespace Monadic.Tests.Effect.Data.InMemory.Integration.IntegrationTests;

public class InMemoryProviderTests : TestSetup
{
    public override ServiceProvider ConfigureServices(IServiceCollection services) =>
        services.AddScopedMonadicWorkflow<ITestWorkflow, TestWorkflow>().BuildServiceProvider();

    [Ignore("Serialization Failing for Input/Output Objects.")]
    public async Task TestInMemoryProviderCanCreateMetadata()
    {
        // Arrange
        var inMemoryContextFactory =
            Scope.ServiceProvider.GetRequiredService<IDataContextProviderFactory>();

        var context = (IDataContext)inMemoryContextFactory.Create();

        var metadata = Metadata.Create(
            new CreateMetadata() { Name = "TestMetadata", Input = Unit.Default }
        );

        await context.Track(metadata);

        await context.SaveChanges(CancellationToken.None);
        context.Reset();

        // Act
        var foundMetadata = await context.Metadatas.FirstOrDefaultAsync(x => x.Id == metadata.Id);

        // Assert
        foundMetadata.Should().NotBeNull();
        foundMetadata.Id.Should().Be(metadata.Id);
        foundMetadata.Name.Should().Be(metadata.Name);
    }

    [Ignore("Serialization Failing for Input/Output Objects.")]
    public async Task TestInMemoryProviderCanRunWorkflow()
    {
        // Arrange

        var workflow = Scope.ServiceProvider.GetRequiredService<ITestWorkflow>();

        // Act
        await workflow.Run(Unit.Default);

        // Assert
        workflow.Metadata.Name.Should().Be("TestWorkflow");
        workflow.Metadata.FailureException.Should().BeNullOrEmpty();
        workflow.Metadata.FailureReason.Should().BeNullOrEmpty();
        workflow.Metadata.FailureStep.Should().BeNullOrEmpty();
        workflow.Metadata.WorkflowState.Should().Be(WorkflowState.Completed);
    }

    private class TestWorkflow : EffectWorkflow<Unit, Unit>, ITestWorkflow
    {
        protected override async Task<Either<Exception, Unit>> RunInternal(Unit input) =>
            Activate(input).Resolve();
    }

    private interface ITestWorkflow : IEffectWorkflow<Unit, Unit> { }
}
