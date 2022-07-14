using Holidays.Configuration;

namespace Holidays.Selenium;

public class OffersDatasourceSettingsDescriptor : ISettingsDescriptor
{
    public string Section => "OffersDatasource";
}

public class OffersDatasourceSettings : ISettings<OffersDatasourceSettingsDescriptor>
{
    public string Provider { get; init; } = null!;
    
    public bool IsValid() => !string.IsNullOrWhiteSpace(Provider);
}
