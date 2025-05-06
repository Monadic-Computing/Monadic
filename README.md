# Monadic

[![Build Status](https://github.com/Theauxm/Monadic/workflows/Release%20NuGet%20Package/badge.svg)](https://github.com/Theauxm/Monadic/actions)
[![Test Status](https://github.com/Theauxm/Monadic/workflows/Monadic:%20Run%20CI/CD%20Test%20Suite/badge.svg)](https://github.com/Theauxm/Monadic/actions)

## Description

Monadic is a .NET library for Railway Oriented Programming, building from functional concepts and attempting to create an encapsulated way of running a piece of code with discrete steps. It aims to simplify complex workflows by providing a clear, linear flow of operations while handling errors and maintaining code readability.

## Features

- **Railway Oriented Programming**: Implements the Railway Oriented Programming paradigm for clear, linear, and maintainable workflows.
- **Functional Concepts**: Leverages functional programming concepts to enhance code clarity and robustness.
- **Encapsulated Steps**: Encapsulates each step of a process, making the code easy to read, write, and maintain.
- **Error Handling**: Built-in mechanisms for handling errors at each step without disrupting the overall flow.
- **Open Source**: Fully open source under the MIT license.

## Installation

You can install Monadic via NuGet. Run the following command in your package manager console:

```sh
dotnet add package Theauxm.Monadic
```

Or, you can add it directly to your `.csproj` file:

```csharp
<PackageReference Include="Theauxm.Monadic" Version="..." />
```

## Available NuGet Packages

Monadic is distributed as several NuGet packages, each providing specific functionality:

| Package | Description | Version |
|---------|-------------|---------|
| [Theauxm.Monadic.Effect](https://www.nuget.org/packages/Theauxm.Monadic.Effect/) | Effects for Monadic Workflows | ![NuGet Version](https://img.shields.io/nuget/v/Theauxm.Monadic.Effect) |
| [Theauxm.Monadic.Effect.Data](https://www.nuget.org/packages/Theauxm.Monadic.Effect.Data/) | Data persistence abstractions for Monadic Effects | ![NuGet Version](https://img.shields.io/nuget/v/Theauxm.Monadic.Effect.Data) |
| [Theauxm.Monadic.Effect.Data.InMemory](https://www.nuget.org/packages/Theauxm.Monadic.Effect.Data.InMemory/) | In-memory data persistence for Monadic Effects | ![NuGet Version](https://img.shields.io/nuget/v/Theauxm.Monadic.Effect.Data.InMemory) |
| [Theauxm.Monadic.Effect.Data.Postgres](https://www.nuget.org/packages/Theauxm.Monadic.Effect.Data.Postgres/) | PostgreSQL data persistence for Monadic Effects | ![NuGet Version](https://img.shields.io/nuget/v/Theauxm.Monadic.Effect.Data.Postgres) |
| [Theauxm.Monadic.Effect.Json](https://www.nuget.org/packages/Theauxm.Monadic.Effect.Json/) | JSON serialization for Monadic Effects | ![NuGet Version](https://img.shields.io/nuget/v/Theauxm.Monadic.Effect.Json) |
| [Theauxm.Monadic.Effect.Mediator](https://www.nuget.org/packages/Theauxm.Monadic.Effect.Mediator/) | Mediator pattern implementation for Monadic Effects | ![NuGet Version](https://img.shields.io/nuget/v/Theauxm.Monadic.Effect.Mediator) |
| [Theauxm.Monadic.Effect.Parameter](https://www.nuget.org/packages/Theauxm.Monadic.Effect.Parameter/) | Parameter serialization for Monadic Effects | ![NuGet Version](https://img.shields.io/nuget/v/Theauxm.Monadic.Effect.Parameter) |

## Usage Examples

### Basic Workflow

A workflow in Monadic represents a sequence of steps that process data in a linear fashion. Here's a basic example of a workflow:

```csharp
using Monadic.Exceptions;
using Monadic.Workflow;
using LanguageExt;

// Define a workflow that takes Ingredients as input and produces a List<GlassBottle> as output
public class Cider
    : Workflow<Ingredients, List<GlassBottle>>,
        ICider
{
    // Implement the RunInternal method to define the workflow steps
    protected override async Task<Either<Exception, List<GlassBottle>>> RunInternal(
        Ingredients input
    ) => Activate(input) 
            .Chain<Prepare>() // Chain steps together, passing the output of one step as input to the next
            .Chain<Ferment>()
            .Chain<Brew>()
            .Chain<Bottle>()
            .Resolve(); // Resolve the final result
}
```

### Step Anatomy

Steps are the building blocks of workflows. Each step performs a specific operation and can be chained together to form a complete workflow. Here's an example of a step:

```csharp
using Monadic.Exceptions;
using Monadic.Step;
using LanguageExt;
using LanguageExt.UnsafeValueAccess;

// Define a step that takes Ingredients as input and produces a BrewingJug as output
public class Prepare : Step<Ingredients, BrewingJug>, IPrepare
{
    // Implement the Run method to define the step's operation
    public override async Task<BrewingJug> Run(Ingredients input)
    {
        const int gallonWater = 1;

        // Perform the step's operation
        var gallonAppleJuice = Boil(gallonWater, input.Apples, input.BrownSugar);

        // Handle errors
        if (gallonAppleJuice < 0)
            throw new Exception("Couldn't make a Gallon of Apple Juice!")

        // Return the result
        return new BrewingJug() { Gallons = gallonAppleJuice, Ingredients = input };
    }

    // Helper method for the step
    private async int Boil(
        int gallonWater,
        int numApples,
        int ozBrownSugar
    ) => gallonWater + (numApples / 8) + (ozBrownSugar / 128);
}
```

### EffectWorkflow

EffectWorkflows extend the basic workflow concept by adding support for effects, which allow for side effects like logging, data persistence, and more. Here's an example of an EffectWorkflow:

```csharp
using Monadic.Effect.Services.EffectWorkflow;
using LanguageExt;

// Define an EffectWorkflow that takes TestEffectWorkflowInput as input and produces TestEffectWorkflow as output
public class ExampleEffectWorkflow
    : EffectWorkflow<WorkflowInput, WorkflowOutput>
        IExampleEffectWorkflow
{
    // Implement the RunInternal method to define the workflow steps
    protected override async Task<Either<Exception, WorkflowOutput>> RunInternal(
        WorkflowInput input
    ) => Activate(input)
        .Chain<StepOne>
        .Chain<StepTwo>
        .Chain<StepThree>
        .Resolve();
}

// Define the interface for the workflow
public interface IExampleEffectWorkflow
    : IEffectWorkflow<WorkflowInput, WorkflowOutput> { }
```

### WorkflowBus

The WorkflowBus provides a way to run workflows from anywhere in your application, such as from controllers, services, or even other workflows. Here are some examples of using the WorkflowBus:

#### Using WorkflowBus in a Step

```csharp
using Monadic.Step;
using Monadic.Effect.Mediator.Services.WorkflowBus;
using Microsoft.Extensions.Logging;
using LanguageExt;

// Define a step that runs another workflow
internal class StepToRunNestedWorkflow(IWorkflowBus workflowBus) : Step<Unit, IInternalWorkflow>
{
    public override async Task<IInternalWorkflow> Run(Unit input)
    {
        // Run a workflow using the WorkflowBus
        var testWorkflow = await workflowBus.RunAsync<IInternalWorkflow>(
            input
        );

        return testWorkflow;
    }
}
```

#### Using WorkflowBus in a Controller

```csharp
using Microsoft.AspNetCore.Mvc;
using Monadic.Effect.Mediator.Services.WorkflowBus;
using Monadic.Exceptions;

[ApiController]
[Route("api/[controller]")]
public class OrdersController(IWorkflowBus _workflowBus) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> CreateOrder(CreateOrderRequest request)
    {
        try
        {
            // Run a workflow using the WorkflowBus
            var order = await _workflowBus.RunAsync<Order>(request);
            return Ok(order);
        }
        catch (WorkflowException ex)
        {
            return BadRequest(ex.Message);
        }
    }
    
    [HttpGet("{id}")]
    public async Task<IActionResult> GetOrder(int id)
    {
        try
        {
            // Run a workflow using the WorkflowBus
            var order = await _workflowBus.RunAsync<Order>(new GetOrderRequest { Id = id });
            return Ok(order);
        }
        catch (WorkflowException ex) when (ex.Message.Contains("not found"))
        {
            return NotFound();
        }
        catch (WorkflowException ex)
        {
            return BadRequest(ex.Message);
        }
    }
}
```

#### Registering WorkflowBus with Dependency Injection

```csharp
using Microsoft.Extensions.DependencyInjection;
using Monadic.Effect.Extensions;

// Register Monadic services with dependency injection
new ServiceCollection()
    .AddMonadicEffects(
        options =>
            options
                .AddEffectWorkflowBus(
                    assemblies:
                    [
                        typeof(AssemblyMarker).Assembly
                    ]
                )
    );
```

## Documentation

For detailed documentation and API references, please visit the [official documentation.](https://github.com/Theauxm/Monadic/wiki)

## Contributing

Contributions are welcome! Please read our Contributing Guide to learn about our development process, how to propose bugfixes and improvements, and how to build and test your changes.

## License

Monadic is licensed under the MIT License.

## Contact

If you have any questions or suggestions, feel free to open an issue.

## Acknowledgements

Without the help and guidance of Mark Keaton this project would not have been possible.
