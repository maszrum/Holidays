using Holidays.Configuration;

namespace Holidays.Selenium;

public class OffersDataSourceSettingsDescriptor : ISettingsDescriptor
{
    public string Section => "OffersDataSource";
}

public class OffersDataSourceSettings : ISettings<OffersDataSourceSettingsDescriptor>
{
    public string Provider { get; init; } = null!;
    
    public int CollectingTimeoutSeconds { get; init; }
    
    public int PauseBetweenCollectionsSeconds { get; init; }
    
    public int NumberOfDaysToCollectOffers { get; init; }
    
    public bool IsValid() => 
        !string.IsNullOrWhiteSpace(Provider) && 
        CollectingTimeoutSeconds > 0 && 
        PauseBetweenCollectionsSeconds > 0 && 
        NumberOfDaysToCollectOffers > 0;
}
