using Monadic.ArrayLogger.Services.ArrayLoggingProvider;
using Monadic.Effect.Data.Extensions;
using Monadic.Effect.Data.Postgres.Extensions;
using Monadic.Effect.Extensions;
using Monadic.Effect.Json.Extensions;
using Monadic.Effect.Mediator.Extensions;
using Monadic.Effect.Mediator.Services.WorkflowBus;
using Monadic.Effect.Parameter.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Monadic.Tests.Effect.Data.Postgres.Integration;

[TestFixture]
public abstract class TestSetup
{
    private ServiceProvider ServiceProvider { get; set; }

    public IServiceScope Scope { get; private set; }

    public IWorkflowBus WorkflowBus { get; private set; }

    [OneTimeSetUp]
    public async Task RunBeforeAnyTests()
    {
        var configuration = new ConfigurationBuilder()
            .SetBasePath(AppContext.BaseDirectory)
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .Build();
        var connectionString = configuration.GetRequiredSection("Configuration")[
            "DatabaseConnectionString"
        ]!;

        var arrayLoggingProvider = new ArrayLoggingProvider();

        ServiceProvider = new ServiceCollection()
            .AddSingleton<ILoggerProvider>(arrayLoggingProvider)
            .AddSingleton<IArrayLoggingProvider>(arrayLoggingProvider)
            .AddLogging()
            .AddMonadicEffects(
                options =>
                    options
                        .AddEffectWorkflowBus(
                            assemblies:
                            [
                                typeof(AssemblyMarker).Assembly,
                                typeof(Monadic.Tests.Effect.Integration.AssemblyMarker).Assembly
                            ]
                        )
                        .SaveWorkflowParameters()
                        .AddPostgresEffect(connectionString)
                        .AddEffectDataContextLogging(minimumLogLevel: LogLevel.Trace)
                        .AddJsonEffect()
            )
            .BuildServiceProvider();
    }

    [OneTimeTearDown]
    public async Task RunAfterAnyTests()
    {
        await ServiceProvider.DisposeAsync();
    }

    [SetUp]
    public virtual async Task TestSetUp()
    {
        Scope = ServiceProvider.CreateScope();
        WorkflowBus = Scope.ServiceProvider.GetRequiredService<IWorkflowBus>();
    }

    [TearDown]
    public async Task TestTearDown()
    {
        Scope.Dispose();
    }
}
