using Monadic.Effect.Data.Services.DataContext;
using Monadic.Step;
using LanguageExt;

namespace Monadic.Effect.Data.Steps.CommitTransaction;

/// <summary>
/// Built-in step allowing for transactions to be committed.
/// </summary>
public class CommitTransaction(IDataContext dataContextFactory) : Step<Unit, Unit>
{
    public override async Task<Unit> Run(Unit input)
    {
        await dataContextFactory.CommitTransaction();

        return Unit.Default;
    }
}
