using Holidays.Configuration;

namespace Holidays.Selenium;

public class SeleniumSettingsDescriptor : ISettingsDescriptor
{
    public string Section => "Selenium";
}

public class SeleniumSettings : ISettings<SeleniumSettingsDescriptor>
{
    public string RemoteWebDriverUrl { get; init; } = null!;

    public int CommandTimeoutSeconds { get; init; }
    
    public bool IsValid() => 
        !string.IsNullOrWhiteSpace(RemoteWebDriverUrl) && CommandTimeoutSeconds > 0;
}
