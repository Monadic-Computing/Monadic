using NUnit.Framework;

namespace Monadic.Tests;

[SetUpFixture]
public class TestFixture
{
    [OneTimeSetUp]
    public async Task RunBeforeAnyTests() { }

    [OneTimeTearDown]
    public async Task RunAfterAnyTests() { }
}
