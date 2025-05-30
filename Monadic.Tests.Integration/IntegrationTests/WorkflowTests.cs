using FluentAssertions;
using LanguageExt;
using Microsoft.Extensions.Logging;
using Monadic.Exceptions;
using Monadic.Step;
using Monadic.Tests.Integration.Examples.Brewery;
using Monadic.Tests.Integration.Examples.Brewery.Steps.Bottle;
using Monadic.Tests.Integration.Examples.Brewery.Steps.Brew;
using Monadic.Tests.Integration.Examples.Brewery.Steps.Ferment;
using Monadic.Tests.Integration.Examples.Brewery.Steps.Prepare;
using Monadic.Workflow;
using Moq;

namespace Monadic.Tests.Integration.IntegrationTests;

using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

public class WorkflowTests : TestSetup
{
    private IBrew _brew;

    public override IServiceProvider ConfigureServices(IServiceCollection services)
    {
        services.AddScoped<ICider, Cider>();
        services.AddScoped<IPrepare, Prepare>();
        services.AddScoped<IBrew, Brew>();
        services.AddScoped<IFerment, Ferment>();
        services.AddScoped<IBottle, Bottle>();

        return services.BuildServiceProvider();
    }

    [SetUp]
    public override async Task TestSetUp()
    {
        await base.TestSetUp();

        _brew = ServiceProvider.GetRequiredService<IBrew>();
    }

    [Theory]
    public async Task TestInputOfTuple()
    {
        // Arrange
        var intInput = 1;
        var stringInput = "hello";
        var objectInput = new object();
        var input = (intInput, stringInput, objectInput);
        var workflow = new WorkflowTestWithTupleInput();

        // Act
        var result = await workflow.Run(input);

        // Assert
        workflow.Exception.Should().BeNull();
        workflow.Memory.Should().NotBeNull();
        workflow.Memory.Count.Should().Be(84);
        result.Should().NotBeNull();

        var (boolResult, doubleResult, objectResult) = result;
        boolResult.Should().BeTrue();
        doubleResult.Should().Be(1);
        objectResult.Should().NotBe(objectInput);
    }

    [Theory]
    public async Task TestExtractProperty()
    {
        // Arrange
        var outerProperty = new OuterProperty()
        {
            OuterString = "hello world",
            InnerProperty = new InnerProperty() { Number = 7 }
        };
        var workflow = new AccessInnerPropertyTypeWorkflow();

        // Act
        var propertyResult = await workflow.Run(outerProperty);

        // Assert
        workflow.Memory.Should().NotBeNull();
        workflow.Exception.Should().BeNull();
        workflow.Memory.Count.Should().Be(3);
        propertyResult.Number.Should().Be(7);
    }

    [Theory]
    public async Task TestExtractField()
    {
        // Arrange
        var outerField = new OuterField()
        {
            OuterString = "hello mars",
            InnerProperty = new InnerField() { Number = 8 }
        };
        var workflow = new AccessInnerFieldTypeWorkflow();

        // Act
        var fieldResult = await workflow.Run(outerField);

        // Assert
        workflow.Memory.Should().NotBeNull();
        workflow.Exception.Should().BeNull();
        fieldResult.Number.Should().Be(8);
    }

    [Theory]
    public async Task TestChain()
    {
        // Arrange
        var prepare = ServiceProvider.GetRequiredService<IPrepare>();
        var bottle = ServiceProvider.GetRequiredService<IBottle>();

        var workflow = new ChainTest(_brew, prepare, bottle);

        var ingredients = new Ingredients()
        {
            Apples = 1,
            BrownSugar = 1,
            Cinnamon = 1,
            Yeast = 1
        };

        // Act
        var result = await workflow.Run(ingredients);

        // Assert
        workflow.Memory.Should().NotBeNull();
        workflow.Exception.Should().BeNull();
        result.Should().NotBeNull();
    }

    [Theory]
    public async Task TestChainWithMockedService()
    {
        // Arrange
        var workflow = new ChainTestWithMockedService();

        var ingredients = new Ingredients()
        {
            Apples = 1,
            BrownSugar = 1,
            Cinnamon = 1,
            Yeast = 1
        };

        // Act
        var result = await workflow.Run(ingredients);

        // Assert
        workflow.Memory.Should().NotBeNull();
        workflow.Exception.Should().BeNull();
        result.Should().NotBeNull();
    }

    [Theory]
    public async Task TestChainWithNoInputs()
    {
        // Arrange
        var workflow = new ChainTestWithNoInputs();

        var ingredients = new Ingredients()
        {
            Apples = 1,
            BrownSugar = 1,
            Cinnamon = 1,
            Yeast = 1
        };

        // Act
        var result = await workflow.Run(ingredients);

        // Assert
        workflow.Exception.Should().BeNull();
        workflow.Memory.Should().NotBeNull();
        result.Should().NotBeNull();
    }

    [Theory]
    public async Task TestChainWithInterfaceTupleArgument()
    {
        // Arrange
        var workflow = new ChainTestWithInterfaceTuple();

        var ingredients = new Ingredients()
        {
            Apples = 1,
            BrownSugar = 1,
            Cinnamon = 1,
            Yeast = 1
        };

        // Act
        var result = await workflow.Run(ingredients);

        // Assert
        workflow.Exception.Should().BeNull();
        workflow.Memory.Should().NotBeNull();
        result.Should().NotBeNull();
    }

    [Theory]
    public async Task TestChainWithOneTypedService()
    {
        // Arrange
        var workflow = new ChainTestWithOneTypedService();

        var ingredients = new Ingredients()
        {
            Apples = 1,
            BrownSugar = 1,
            Cinnamon = 1,
            Yeast = 1
        };

        // Act
        var result = await workflow.Run(ingredients);

        // Assert
        workflow.Exception.Should().BeNull();
        workflow.Memory.Should().NotBeNull();
        result.Should().NotBeNull();
    }

    [Theory]
    public async Task TestChainWithTwoTypedService()
    {
        // Arrange
        var workflow = new ChainTestWithTwoTypedServices();

        var ingredients = new Ingredients()
        {
            Apples = 1,
            BrownSugar = 1,
            Cinnamon = 1,
            Yeast = 1
        };

        // Act
        var result = await workflow.Run(ingredients);

        // Assert
        workflow.Exception.Should().BeNull();
        workflow.Memory.Should().NotBeNull();
        result.Should().NotBeNull();
    }

    [Theory]
    public async Task TestChainWithShortCircuit()
    {
        // Arrange
        var prepare = ServiceProvider.GetRequiredService<IPrepare>();
        var ferment = ServiceProvider.GetRequiredService<IFerment>();

        var workflow = new ChainTestWithShortCircuit(prepare, ferment);

        var ingredients = new Ingredients()
        {
            Apples = 1,
            BrownSugar = 1,
            Cinnamon = 1,
            Yeast = 1
        };

        // Act
        var result = await workflow.Run(ingredients);

        // Assert
        workflow.Exception.Should().BeNull();
        workflow.Memory.Should().NotBeNull();
        result.Should().NotBeNull();
    }

    [Theory]
    public async Task TestChainWithUnitInput()
    {
        // Arrange
        var workflow = new ChainTestWithUnitInput();

        var ingredients = new Ingredients()
        {
            Apples = 1,
            BrownSugar = 1,
            Cinnamon = 1,
            Yeast = 1
        };

        // Act
        var result = await workflow.Run(ingredients);

        // Assert
        workflow.Exception.Should().BeNull();
        workflow.Memory.Should().NotBeNull();
        result.Should().NotBeNull();
    }

    [Theory]
    public async Task TestChainWithShortCircuitStaysLeft()
    {
        // Arrange
        var prepare = ServiceProvider.GetRequiredService<IPrepare>();
        var ferment = ServiceProvider.GetRequiredService<IFerment>();

        var workflow = new ChainTestWithShortCircuitStaysLeft(prepare, ferment);

        var ingredients = new Ingredients()
        {
            Apples = 1,
            BrownSugar = 1,
            Cinnamon = 1,
            Yeast = 1
        };

        // Act
        var result = await workflow.Run(ingredients);

        // Assert
        workflow.Exception.Should().BeNull();
        workflow.Memory.Should().NotBeNull();
        result.Should().NotBeNull();
    }

    [Theory]
    public async Task TestWithException()
    {
        // Arrange
        var workflow = new ChainTestWithException();

        // Act
        // Assert

        Assert.ThrowsAsync<WorkflowException>(async () => await workflow.Run(Unit.Default));
    }

    [Theory]
    public async Task TestWithLoggerProvider()
    {
        // Arrange
        var loggerProvider = LoggerFactory.Create(builder => builder.AddConsole());

        var testService = new TestService();

        var workflow = new ChainTestWithLoggerProvider(loggerProvider, testService);

        // Act
        var result = await workflow.Run(Unit.Default);

        // Assert
        result.Should().Be(Unit.Default);
    }

    [Theory]
    public async Task TestWithServiceProvider()
    {
        // Arrange
        var serviceProvider = new ServiceCollection()
            .AddLogging(x => x.AddConsole())
            .AddScoped<ITestService, TestService>()
            .BuildServiceProvider();

        var workflow = new ChainTestWithServiceProvider();

        // Act
        var result = await workflow.Run(Unit.Default, serviceProvider);

        // Assert
        result.Should().Be(Unit.Default);
    }

    [Theory]
    public async Task TestWithMultipleInheritedInterface()
    {
        // Arrange
        var inheritedObject = new InheritedObject();

        var workflow = new MemoryInterfaceTest();

        // Act
        var result = await workflow.Run(inheritedObject);

        // Assert
        result.Should().Be(Unit.Default);
    }

    private class ThrowsStep : Step<Unit, Unit>
    {
        public override Task<Unit> Run(Unit input) =>
            throw new WorkflowException("This is a workflow exception.");
    }

    private class OuterProperty
    {
        public string OuterString { get; set; }

        public InnerProperty InnerProperty { get; set; }
    }

    private class InnerProperty
    {
        public int Number { get; set; }
    }

    private class OuterField
    {
        public string OuterString;

        public InnerField InnerProperty;
    }

    private class InnerField
    {
        public int Number;
    }

    private class AccessInnerPropertyTypeWorkflow : Workflow<OuterProperty, InnerProperty>
    {
        protected override async Task<Either<Exception, InnerProperty>> RunInternal(
            OuterProperty input
        ) => Activate(input).Extract<OuterProperty, InnerProperty>().Resolve();
    }

    private class AccessInnerFieldTypeWorkflow : Workflow<OuterField, InnerField>
    {
        protected override async Task<Either<Exception, InnerField>> RunInternal(
            OuterField input
        ) => Activate(input).Extract<OuterField, InnerField>().Resolve();
    }

    private class TwoTupleStepTest : Step<(Ingredients, BrewingJug), Unit>
    {
        public override async Task<Unit> Run((Ingredients, BrewingJug) input)
        {
            var (x, y) = input;

            x.Apples++;
            y.Gallons++;

            return Unit.Default;
        }
    }

    /// <summary>
    /// Tests to ensure that tuple casting to interface works correctly
    /// </summary>
    private class TwoTupleStepInterfaceTest : Step<(Ingredients, IBrewingJug), Unit>
    {
        public override async Task<Unit> Run((Ingredients, IBrewingJug) input)
        {
            var (x, y) = input;

            x.Apples++;
            y.Gallons++;

            return Unit.Default;
        }
    }

    private class CastBrewingJug : Step<IBrewingJug, BrewingJug>
    {
        public override async Task<BrewingJug> Run(IBrewingJug input)
        {
            // MAGIC!
            return (BrewingJug)input;
        }
    }

    private class TupleReturnStep : Step<Unit, (bool, double, object)>
    {
        public override async Task<(bool, double, object)> Run(Unit input)
        {
            return (true, 1, new object());
        }
    }

    private class ThreeTupleStepTest : Step<(Ingredients, BrewingJug, Unit), Unit>
    {
        public override async Task<Unit> Run((Ingredients, BrewingJug, Unit) input)
        {
            var (x, y, z) = input;

            x.Apples++;
            y.Gallons++;

            return Unit.Default;
        }
    }

    private interface IFirstInheritedInterface { }

    private interface ISecondInheritedInterface { }

    private class InheritedObject : IFirstInheritedInterface, ISecondInheritedInterface { }

    private class TestMemoryStep : Step<ISecondInheritedInterface, Unit>
    {
        public override async Task<Unit> Run(ISecondInheritedInterface input)
        {
            return Unit.Default;
        }
    }

    private interface ITestService { }

    private class TestService : ITestService { }

    private class LoggerTest(ILogger<LoggerTest> logger, ITestService testService)
        : Step<Unit, Unit>
    {
        public override async Task<Unit> Run(Unit input)
        {
            logger.LogInformation("In LoggerTest");

            return Unit.Default;
        }
    }

    private class ChainTest(IBrew brew, IPrepare prepare, IBottle bottle)
        : Workflow<Ingredients, List<GlassBottle>>
    {
        protected override async Task<Either<Exception, List<GlassBottle>>> RunInternal(
            Ingredients input
        ) =>
            Activate(input, "this is a test string to make sure it gets added to memory")
                .Chain<IPrepare, Ingredients, BrewingJug>(prepare)
                .Chain<Ferment, BrewingJug>()
                .Chain<TwoTupleStepTest, (Ingredients, BrewingJug)>()
                .Chain<ThreeTupleStepTest, (Ingredients, BrewingJug, Unit)>()
                .Chain<IBrew, BrewingJug>(brew)
                .Chain<IBottle, BrewingJug, List<GlassBottle>>(bottle)
                .Resolve();
    }

    private class MemoryInterfaceTest : Workflow<IFirstInheritedInterface, Unit>
    {
        protected override async Task<Either<Exception, Unit>> RunInternal(
            IFirstInheritedInterface input
        ) => Activate(input).Chain<TestMemoryStep>().Resolve();
    }

    private class ChainTestWithNoInputs : Workflow<Ingredients, List<GlassBottle>>
    {
        protected override async Task<Either<Exception, List<GlassBottle>>> RunInternal(
            Ingredients input
        )
        {
            var brew = new Brew();
            var ferment = new Ferment() as IFerment;
            return Activate(input, "this is a test string to make sure it gets added to memory")
                .AddServices(ferment)
                .Chain<Prepare>()
                .Chain<Ferment>()
                .Chain<TwoTupleStepTest>()
                .Chain<ThreeTupleStepTest>()
                .Chain(brew)
                .Chain<Bottle>()
                .Resolve();
        }
    }

    private class ChainTestWithInterfaceTuple : Workflow<Ingredients, List<GlassBottle>>
    {
        protected override async Task<Either<Exception, List<GlassBottle>>> RunInternal(
            Ingredients input
        )
        {
            var brew = new Brew();
            var ferment = new Ferment() as IFerment;
            return Activate(input, "this is a test string to make sure it gets added to memory")
                .AddServices(ferment)
                .Chain<PrepareWithInterface>()
                .Chain<TwoTupleStepInterfaceTest>() // What we're really testing here
                .Chain<CastBrewingJug>()
                .Chain<Ferment>()
                .Chain<ThreeTupleStepTest>()
                .Chain(brew)
                .Chain<Bottle>()
                .Resolve();
        }
    }

    private class ChainTestWithOneTypedService : Workflow<Ingredients, List<GlassBottle>>
    {
        protected override async Task<Either<Exception, List<GlassBottle>>> RunInternal(
            Ingredients input
        )
        {
            var brew = new Brew();
            var ferment = new Ferment();

            // IFerment implements IStep and IFerment
            // Normally, AddServices looks for the First Interface that is not IStep
            // This uses a Type argument to do a Service addition to find IFerment
            // (which is actually the second interface that it implements)

            return Activate(input, "this is a test string to make sure it gets added to memory")
                .AddServices<IFerment>(ferment)
                .Chain<Prepare>()
                .IChain<IFerment>()
                .Chain<TwoTupleStepTest>()
                .Chain<ThreeTupleStepTest>()
                .Chain(brew)
                .Chain<Bottle>()
                .Resolve();
        }
    }

    private class ChainTestWithTwoTypedServices : Workflow<Ingredients, List<GlassBottle>>
    {
        protected override async Task<Either<Exception, List<GlassBottle>>> RunInternal(
            Ingredients input
        )
        {
            var brew = new Brew();
            var ferment = new Ferment();
            var prepare = new Prepare(ferment);

            // IFerment implements IStep and IFerment
            // Normally, AddServices looks for the First Interface that is not IStep
            // This uses a Type argument to do a Service addition to find IFerment
            // (which is actually the second interface that it implements)

            return Activate(input, "this is a test string to make sure it gets added to memory")
                .AddServices<IPrepare, IFerment>(prepare, ferment)
                .IChain<IPrepare>()
                .IChain<IFerment>()
                .Chain<TwoTupleStepTest>()
                .Chain<ThreeTupleStepTest>()
                .Chain(brew)
                .Chain<Bottle>()
                .Resolve();
        }
    }

    private class ChainTestWithMockedService : Workflow<Ingredients, List<GlassBottle>>
    {
        protected override async Task<Either<Exception, List<GlassBottle>>> RunInternal(
            Ingredients input
        )
        {
            var brew = new Brew();
            var ferment = new Mock<IFerment>().Object;
            return Activate(input, "this is a test string to make sure it gets added to memory")
                .AddServices(ferment)
                .Chain<Prepare>()
                .Chain<Ferment>()
                .Chain<TwoTupleStepTest>()
                .Chain<ThreeTupleStepTest>()
                .Chain(brew)
                .Chain<Bottle>()
                .Resolve();
        }
    }

    private class WorkflowTestWithTupleInput
        : Workflow<(int, string, object), (bool, double, object)>
    {
        protected override async Task<Either<Exception, (bool, double, object)>> RunInternal(
            (int, string, object) input
        ) => Activate(input).Chain<TupleReturnStep>().ShortCircuit<TupleReturnStep>().Resolve();
    }

    private class ChainTestWithUnitInput : Workflow<Ingredients, List<GlassBottle>>
    {
        protected override async Task<Either<Exception, List<GlassBottle>>> RunInternal(
            Ingredients input
        )
        {
            var brew = new Brew();
            var ferment = new Ferment() as IFerment;
            return Activate(input, "this is a test string to make sure it gets added to memory")
                .AddServices(ferment)
                .Chain<Meditate>()
                .Chain<Prepare>()
                .Chain<Ferment>()
                .Chain<TwoTupleStepTest>()
                .Chain<ThreeTupleStepTest>()
                .Chain(brew)
                .Chain<Bottle>()
                .Resolve();
        }
    }

    private class ChainTestWithShortCircuit(IPrepare prepare, IFerment ferment)
        : Workflow<Ingredients, List<GlassBottle>>
    {
        protected override async Task<Either<Exception, List<GlassBottle>>> RunInternal(
            Ingredients input
        )
        {
            var brew = new Brew();
            return Activate(input)
                .AddServices(prepare, ferment)
                .IChain<IPrepare>()
                .Chain<Ferment>()
                .Chain<TwoTupleStepTest>()
                .Chain<ThreeTupleStepTest>()
                .ShortCircuit<StealCinnamonAndRunAway>()
                .Chain(brew)
                .Chain<Bottle>()
                .Resolve();
        }
    }

    private class ChainTestWithShortCircuitStaysLeft(IPrepare prepare, IFerment ferment)
        : Workflow<Ingredients, List<GlassBottle>>
    {
        protected override async Task<Either<Exception, List<GlassBottle>>> RunInternal(
            Ingredients input
        )
        {
            var brew = new Brew();
            return Activate(input)
                .AddServices(prepare, ferment)
                .IChain<IPrepare>()
                .ShortCircuit<TripTryingToSteal>()
                .Chain<Ferment>()
                .Chain<TwoTupleStepTest>()
                .Chain<ThreeTupleStepTest>()
                .Chain(brew)
                .Chain<Bottle>()
                .Resolve();
        }
    }

    private class ChainTestWithException : Workflow<Unit, Unit>
    {
        protected override async Task<Either<Exception, Unit>> RunInternal(Unit input) =>
            Activate(input).Chain<ThrowsStep>().Resolve();
    }

    private class ChainTestWithLoggerProvider(
        ILoggerFactory loggerFactory,
        ITestService testService
    ) : Workflow<Unit, Unit>
    {
        protected override async Task<Either<Exception, Unit>> RunInternal(Unit input) =>
            Activate(input).AddServices(loggerFactory, testService).Chain<LoggerTest>().Resolve();
    }

    private class ChainTestWithServiceProvider : Workflow<Unit, Unit>
    {
        protected override async Task<Either<Exception, Unit>> RunInternal(Unit input) =>
            Activate(input).Chain<LoggerTest>().Resolve();
    }
}
