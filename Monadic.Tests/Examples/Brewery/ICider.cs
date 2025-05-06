using Monadic.Tests.Examples.Brewery.Steps.Bottle;
using Monadic.Tests.Examples.Brewery.Steps.Prepare;
using Monadic.Workflow;

namespace Monadic.Tests.Examples.Brewery;

public interface ICider : IWorkflow<Ingredients, List<GlassBottle>> { }
