using System.Transactions;

namespace Holidays.InMemoryStore.Tests;

public abstract class DatabaseTestsBase
{
    protected InMemoryDatabase Database { get; } = InMemoryDatabase.CreateEmpty();
    
    protected async Task<T> DoWithTransactionAndRollback<T>(Func<InMemoryDatabase, Task<T>> action)
    {
        using var transactionScope = new TransactionScope();
        return await action(Database);
    }
    
    protected T DoWithTransactionAndRollback<T>(Func<InMemoryDatabase, T> action)
    {
        using var transactionScope = new TransactionScope();
        return action(Database);
    }

    protected async Task DoWithTransactionAndRollback(Func<InMemoryDatabase, Task> action)
    {
        using var transactionScope = new TransactionScope();
        await action(Database);
    }

    protected void DoWithTransactionAndRollback(Action<InMemoryDatabase> action)
    {
        using var transactionScope = new TransactionScope();
        action(Database);
    }
}
