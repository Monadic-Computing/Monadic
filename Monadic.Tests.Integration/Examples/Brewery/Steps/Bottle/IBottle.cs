using Monadic.Step;
using Monadic.Tests.Integration.Examples.Brewery.Steps.Prepare;

namespace Monadic.Tests.Integration.Examples.Brewery.Steps.Bottle;

public interface IBottle : IStep<BrewingJug, List<GlassBottle>> { }
