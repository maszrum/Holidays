using Dapper;
using Npgsql;

namespace Holidays.Postgres.Initialization;

public class DatabaseInitializer
{
    private readonly PostgresConnectionFactory _connectionFactory;

    public DatabaseInitializer(PostgresConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task InitializeIfNeed()
    {
        var sqlSource = new EmbeddedResourcesSqlSource();
        var expectedTables = sqlSource.GetAvailableTableNames().ToArray();

        await using var connection = await _connectionFactory.CreateConnection();
        await using var transaction = await connection.BeginTransactionAsync();

        try
        {
            await EnsureSchemaCreated(connection, transaction);

            var existingTables = await GetExistingTables(connection, transaction);

            foreach (var expectedTable in expectedTables)
            {
                if (!existingTables.Contains(expectedTable))
                {
                    var query = await sqlSource.ReadSqlForTable(expectedTable);

                    _ = await connection.ExecuteAsync(
                        sql: query,
                        transaction: transaction);
                }
            }

            await transaction.CommitAsync();
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    public async Task InitializeForcefully()
    {
        var sqlSource = new EmbeddedResourcesSqlSource();
        var expectedTables = sqlSource.GetAvailableTableNames().ToArray();

        await using var connection = await _connectionFactory.CreateConnection();
        await using var transaction = await connection.BeginTransactionAsync();

        try
        {
            var existingTables = await GetExistingTables(connection, transaction);

            // drop all tables in reverse order
            foreach (var expectedTable in expectedTables.Reverse())
            {
                if (existingTables.Contains(expectedTable))
                {
                    await DropTable(expectedTable, connection, transaction);
                }
            }

            await EnsureSchemaCreated(connection, transaction);

            // create all tables
            foreach (var expectedTable in expectedTables)
            {
                var query = await sqlSource.ReadSqlForTable(expectedTable);

                _ = await connection.ExecuteAsync(
                    sql: query,
                    transaction: transaction);
            }

            await transaction.CommitAsync();
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    private static async Task EnsureSchemaCreated(
        NpgsqlConnection connection,
        NpgsqlTransaction transaction)
    {
        await connection.ExecuteAsync(
            sql: "CREATE SCHEMA IF NOT EXISTS holidays",
            transaction: transaction);
    }

    private static async Task<IReadOnlyList<string>> GetExistingTables(
        NpgsqlConnection connection,
        NpgsqlTransaction transaction)
    {
        var existingTables = await connection.QueryAsync<string>(
            sql: "SELECT tablename FROM pg_tables WHERE schemaname = 'holidays'",
            transaction: transaction);

        return existingTables.ToArray();
    }

    private static async Task DropTable(
        string tableName,
        NpgsqlConnection connection,
        NpgsqlTransaction transaction)
    {
        await connection.ExecuteAsync(
            sql: $"DROP TABLE holidays.{tableName}",
            transaction: transaction);
    }
}
