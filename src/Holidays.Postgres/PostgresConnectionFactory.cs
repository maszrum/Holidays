using Dapper;
using Npgsql;

namespace Holidays.Postgres;

public class PostgresConnectionFactory
{
    private readonly PostgresSettings _settings;

    static PostgresConnectionFactory()
    {
        DefaultTypeMap.MatchNamesWithUnderscores = true;
    }

    public PostgresConnectionFactory(PostgresSettings settings)
    {
        _settings = settings;
    }

    public async Task<NpgsqlConnection> CreateConnection(CancellationToken cancellationToken = default)
    {
        var connection = new NpgsqlConnection(_settings.ConnectionString);
        await connection.OpenAsync(cancellationToken);

        return connection;
    }
}
