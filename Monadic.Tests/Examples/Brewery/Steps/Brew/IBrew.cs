using Monadic.Step;
using Monadic.Tests.Examples.Brewery.Steps.Prepare;
using LanguageExt;

namespace Monadic.Tests.Examples.Brewery.Steps.Brew;

public interface IBrew : IStep<BrewingJug, Unit> { }
