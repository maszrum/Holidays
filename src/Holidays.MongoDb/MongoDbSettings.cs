using Holidays.Configuration;

namespace Holidays.MongoDb;

public class MongoDbSettingsDescriptor : ISettingsDescriptor
{
    public string Section => "MongoDb";
}

public class MongoDbSettings : ISettings<MongoDbSettingsDescriptor>
{
    public string ConnectionString { get; init; } = null!;

    public string Database { get; init; } = null!;

    public bool IsValid() => 
        !string.IsNullOrWhiteSpace(ConnectionString) && !string.IsNullOrWhiteSpace(Database);
}
