using FluentAssertions;
using LanguageExt;
using Monadic.Extensions;
using Monadic.Step;
using Monadic.Workflow;

namespace Monadic.Tests.Unit.UnitTests.Extensions;

public class InitializeStepTests : TestSetup
{
    [Theory]
    public async Task TestInitializeStep()
    {
        // Arrange
        var input = 1;

        var workflow = new TestWorkflow();

        // Act
        var step = workflow.InitializeStep<TestValidStep, int, string>();

        // Assert
        step.Should().NotBeNull();

        var result = await step!.Run(input);
        result.Should().Be(input.ToString());
    }

    [Theory]
    public async Task TestInvalidInitializeStep()
    {
        // Arrange
        var workflow = new TestWorkflow().Activate(1);

        // Act
        var step = workflow.InitializeStep<TestInvalidStep, int, string>();

        // Assert
        workflow.Exception.Should().NotBeNull();
    }

    private class TestWorkflow : Workflow<int, string>
    {
        protected override async Task<Either<Exception, string>> RunInternal(int input) =>
            Resolve();
    }

    private class TestValidStep : Step<int, string>
    {
        public override async Task<string> Run(int input)
        {
            return input.ToString();
        }
    }

    private class TestInvalidStep(int intInput, string stringInput) { }
}
