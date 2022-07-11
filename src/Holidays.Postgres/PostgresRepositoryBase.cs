using System.Runtime.CompilerServices;
using Npgsql;

namespace Holidays.Postgres;

public abstract class PostgresRepositoryBase : IAsyncDisposable
{
    private readonly bool _externalTransaction;

    protected PostgresRepositoryBase(NpgsqlConnection connection)
    {
        Connection = connection;
    }

    protected PostgresRepositoryBase(NpgsqlConnection connection, NpgsqlTransaction transaction)
    {
        Connection = connection;
        Transaction = transaction;
        _externalTransaction = true;
    }
    
    protected NpgsqlConnection Connection { get; }

    protected NpgsqlTransaction? Transaction { get; private set; }
    
    [MethodImpl(MethodImplOptions.Synchronized)]
    protected Task<NpgsqlTransaction> GetOrCreateTransaction()
    {
        return Transaction is null 
            ? StartAndSet() 
            : Task.FromResult(Transaction);

        async Task<NpgsqlTransaction> StartAndSet()
        {
            Transaction = await Connection.BeginTransactionAsync();
            return Transaction;
        }
    }

    protected virtual async ValueTask DisposeAsync(bool disposing)
    {
        if (disposing)
        {
            if (!_externalTransaction && Transaction is not null)
            {
                await Transaction.DisposeAsync();
            }
        }
    }

    public async ValueTask DisposeAsync()
    {
        await DisposeAsync(true);
        GC.SuppressFinalize(this);
    }
}
