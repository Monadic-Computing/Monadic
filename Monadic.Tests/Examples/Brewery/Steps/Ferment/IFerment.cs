using LanguageExt;
using Monadic.Step;
using Monadic.Tests.Examples.Brewery.Steps.Prepare;

namespace Monadic.Tests.Examples.Brewery.Steps.Ferment;

public interface IFerment : IStep<BrewingJug, Unit> { }
