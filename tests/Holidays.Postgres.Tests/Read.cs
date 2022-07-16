using Holidays.Configuration;

namespace Holidays.Postgres.Tests;

internal static class Read
{
    public static PostgresSettings PostgresSettings()
    {
        var configuration = new ApplicationConfiguration(
            "testsettings.json",
            environmentJsonFileName: default,
            overrideWithEnvironmentVariables: true);

        var settings = configuration.Get<PostgresSettings>();
        return settings;
    }
}
