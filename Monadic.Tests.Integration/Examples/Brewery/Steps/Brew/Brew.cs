using LanguageExt;
using Monadic.Exceptions;
using Monadic.Step;
using Monadic.Tests.Integration.Examples.Brewery.Steps.Prepare;

namespace Monadic.Tests.Integration.Examples.Brewery.Steps.Brew;

public class Brew : Step<BrewingJug, Unit>, IBrew
{
    public override async Task<Unit> Run(BrewingJug input)
    {
        if (!input.IsFermented)
            throw new WorkflowException("We cannot brew our Cider before it is fermented!");

        // Pretend that we waited 2 days...
        input.IsBrewed = true;

        return Unit.Default;
    }
}
