using Holidays.Postgres.Initialization;
using Npgsql;
using NUnit.Framework;

namespace Holidays.Postgres.Tests;

public abstract class DatabaseTestsBase
{
    private bool _dirtyDatabase;

    protected PostgresConnectionFactory ConnectionFactory { get; private set; } = null!;

    [OneTimeSetUp]
    public async Task EnsureDatabaseIsReadyForTests()
    {
        var settings = Read.PostgresSettings();
        ConnectionFactory = new PostgresConnectionFactory(settings);

        var initializer = new DatabaseInitializer(ConnectionFactory);
        await initializer.InitializeForcefully();
    }

    [SetUp]
    public async Task EnsureDatabaseIsNotDirty()
    {
        if (_dirtyDatabase)
        {
            _dirtyDatabase = false;

            var initializer = new DatabaseInitializer(ConnectionFactory);
            await initializer.InitializeForcefully();
        }
    }

    protected async Task<T> DoWithTransactionAndRollback<T>(Func<NpgsqlConnection, NpgsqlTransaction, Task<T>> action)
    {
        await using var connection = await ConnectionFactory.CreateConnection();
        await using var transaction = await connection.BeginTransactionAsync();

        try
        {
            return await action(connection, transaction);
        }
        finally
        {
            await transaction.RollbackAsync();
        }
    }

    protected async Task DoWithTransactionAndRollback(Func<NpgsqlConnection, NpgsqlTransaction, Task> action)
    {
        await using var connection = await ConnectionFactory.CreateConnection();
        await using var transaction = await connection.BeginTransactionAsync();

        try
        {
            await action(connection, transaction);
        }
        finally
        {
            await transaction.RollbackAsync();
        }
    }

    protected void MarkDatabaseAsDirty()
    {
        _dirtyDatabase = true;
    }
}
