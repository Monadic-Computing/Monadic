using LanguageExt;
using Monadic.Effect.Data.Services.DataContext;
using Monadic.Step;

namespace Monadic.Effect.Data.Steps.BeginTransaction;

/// <summary>
/// Built-in step allowing for transactions to occur.
/// </summary>
public class BeginTransaction(IDataContext dataContext) : Step<Unit, Unit>
{
    public override async Task<Unit> Run(Unit input)
    {
        await dataContext.BeginTransaction();

        return Unit.Default;
    }
}
