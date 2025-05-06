using LanguageExt;
using LanguageExt.UnsafeValueAccess;
using Monadic.Exceptions;
using Monadic.Step;
using static LanguageExt.Prelude;

namespace Monadic.Tests.Examples.Brewery.Steps.Prepare;

internal class Meditate : Step<Unit, Unit>
{
    public override async Task<Unit> Run(Unit input)
    {
        // You silently consider what you should brew
        return unit;
    }
}
