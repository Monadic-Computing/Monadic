using LanguageExt;
using Monadic.Step;
using Monadic.Tests.Examples.Brewery.Steps.Prepare;

namespace Monadic.Tests.Examples.Brewery.Steps.Brew;

public interface IBrew : IStep<BrewingJug, Unit> { }
