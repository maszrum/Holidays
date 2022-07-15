namespace Holidays.Configuration.Tests;

internal class TestSettingsDescriptor : ISettingsDescriptor
{
    public string Section => "TestSection";
}

internal class TestSettings : ISettings<TestSettingsDescriptor>
{
    public string StringSetting { get; init; } = null!;

    public int IntSetting { get; init; } = int.MinValue;

    public bool BoolSetting { get; init; }

    public bool IsValid()
    {
        // ReSharper disable once ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
        return StringSetting is not null && IntSetting != int.MinValue;
    }
}
