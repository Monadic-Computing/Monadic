using Monadic.Step;
using Monadic.Tests.Examples.Brewery.Steps.Prepare;
using LanguageExt;

namespace Monadic.Tests.Examples.Brewery.Steps.Ferment;

public interface IFerment : IStep<BrewingJug, Unit> { }
