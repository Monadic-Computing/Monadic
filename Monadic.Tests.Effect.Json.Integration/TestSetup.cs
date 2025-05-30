using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Monadic.ArrayLogger.Services.ArrayLoggingProvider;
using Monadic.Effect.Extensions;
using Monadic.Effect.Json.Extensions;

namespace Monadic.Tests.Effect.Json.Integration;

public abstract class TestSetup
{
    private ServiceProvider ServiceProvider { get; set; }

    public IServiceScope Scope { get; private set; }

    private ServiceCollection ServiceCollection { get; set; }

    [OneTimeSetUp]
    public async Task RunBeforeAnyTests()
    {
        ServiceCollection = new ServiceCollection();

        var arrayProvider = new ArrayLoggingProvider();

        ServiceCollection
            .AddSingleton<IArrayLoggingProvider>(arrayProvider)
            .AddLogging(x => x.AddProvider(arrayProvider))
            .AddMonadicEffects(options => options.AddJsonEffect());

        ServiceProvider = ConfigureServices(ServiceCollection);
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
    }

    [TearDown]
    public async Task TestTearDown()
    {
        Scope.Dispose();
    }

    public abstract ServiceProvider ConfigureServices(IServiceCollection services);
}
