using Monadic.Tests.Integration.Examples.Brewery.Steps.Bottle;
using Monadic.Tests.Integration.Examples.Brewery.Steps.Prepare;
using Monadic.Workflow;

namespace Monadic.Tests.Integration.Examples.Brewery;

public interface ICider : IWorkflow<Ingredients, List<GlassBottle>> { }
