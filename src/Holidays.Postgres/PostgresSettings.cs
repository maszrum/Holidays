using Holidays.Configuration;

namespace Holidays.Postgres;

public class PostgresSettingsDescriptor : ISettingsDescriptor
{
    public string Section => "Postgres";
}

public class PostgresSettings : ISettings<PostgresSettingsDescriptor>
{
    public string ConnectionString { get; init; } = null!;

    public bool IsValid() => !string.IsNullOrWhiteSpace(ConnectionString);
}
