namespace Monadic.Tests;

public interface ITestFixture
{
    Task RunBeforeAnyTests();

    Task RunAfterAnyTests();
}
