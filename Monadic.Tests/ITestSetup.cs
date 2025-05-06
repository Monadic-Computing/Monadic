namespace Monadic.Tests;

public interface ITestSetup
{
    Task TestSetUp();

    Task TestTearDown();
}
