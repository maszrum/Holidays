using Holidays.Configuration;

namespace Holidays.MongoDb.Tests;

internal static class Create
{
    private const string FileName = "testsettings.json";

    public static MongoDbSettings Settings()
    {
        var configuration = new ApplicationConfiguration(
            FileName,
            overrideWithEnvironmentVariables: true);

        var settings = configuration.Get<MongoDbSettings>();

        return settings;
    }

    public static OffersMongoRepository Repository()
    {
        var settings = Settings();
        var connectionFactory = new ConnectionFactory(settings);
        return new OffersMongoRepository(connectionFactory);
    }
}
