using LanguageExt;
using Monadic.Step;
using Monadic.Tests.Integration.Examples.Brewery.Steps.Prepare;

namespace Monadic.Tests.Integration.Examples.Brewery.Steps.Ferment;

public interface IFerment : IStep<BrewingJug, Unit> { }
