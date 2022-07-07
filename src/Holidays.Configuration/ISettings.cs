namespace Holidays.Configuration;

public interface ISettings
{
    bool IsValid();
}

// ReSharper disable once UnusedTypeParameter

public interface ISettings<TDescriptor> : ISettings
    where TDescriptor : ISettingsDescriptor, new()
{
}
