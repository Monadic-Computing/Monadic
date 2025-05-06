using LanguageExt;
using Monadic.Step;
using Monadic.Tests.Integration.Examples.Brewery.Steps.Prepare;

namespace Monadic.Tests.Integration.Examples.Brewery.Steps.Brew;

public interface IBrew : IStep<BrewingJug, Unit> { }
