using LanguageExt;
using Monadic.Exceptions;
using Monadic.Tests.Examples.Brewery.Steps.Bottle;
using Monadic.Tests.Examples.Brewery.Steps.Brew;
using Monadic.Tests.Examples.Brewery.Steps.Ferment;
using Monadic.Tests.Examples.Brewery.Steps.Prepare;
using Monadic.Workflow;

namespace Monadic.Tests.Examples.Brewery;

public class Cider(IPrepare prepare, IFerment ferment, IBrew brew, IBottle bottle)
    : Workflow<Ingredients, List<GlassBottle>>,
        ICider
{
    protected override async Task<Either<Exception, List<GlassBottle>>> RunInternal(
        Ingredients input
    ) =>
        this.Chain<IPrepare, Ingredients, BrewingJug>(prepare, input, out var jug)
            .Chain<IFerment, BrewingJug>(ferment, jug)
            .Chain<IBrew, BrewingJug>(brew, jug)
            .Chain<IBottle, BrewingJug, List<GlassBottle>>(bottle, jug, out var bottles)
            .Resolve(bottles);
}
