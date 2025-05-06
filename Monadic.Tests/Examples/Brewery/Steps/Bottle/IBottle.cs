using Monadic.Step;
using Monadic.Tests.Examples.Brewery.Steps.Prepare;

namespace Monadic.Tests.Examples.Brewery.Steps.Bottle;

public interface IBottle : IStep<BrewingJug, List<GlassBottle>> { }
