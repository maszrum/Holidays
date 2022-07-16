using Microsoft.Extensions.Configuration;

namespace Holidays.Configuration;

public class ApplicationConfiguration
{
    private readonly Lazy<IConfigurationRoot> _configuration;
    private readonly Dictionary<Type, string> _sectionNames = new();
    private readonly ReaderWriterLockSlim _sectionNamesLock = new();

    public ApplicationConfiguration(
        string jsonFileName,
        string? environmentJsonFileName,
        bool overrideWithEnvironmentVariables)
    {
        var configurationBuilder = new ConfigurationBuilder()
            .AddJsonFile(
                jsonFileName,
                optional: false,
                reloadOnChange: false);

        if (environmentJsonFileName is not null)
        {
            AddEnvironmentJsonFile(configurationBuilder, environmentJsonFileName);
        }

        if (overrideWithEnvironmentVariables)
        {
            configurationBuilder.AddEnvironmentVariables();
        }

        _configuration = new Lazy<IConfigurationRoot>(() => configurationBuilder.Build());
    }

    public ApplicationConfiguration(IConfigurationRoot configurationRoot)
    {
        _configuration = new Lazy<IConfigurationRoot>(configurationRoot);
    }

    public IConfigurationRoot ConfigurationRoot => _configuration.Value;

    public TSettings Get<TSettings>() where TSettings : ISettings
    {
        var sectionName = GetSectionName(typeof(TSettings));

        var settings = _configuration.Value
            .GetSection(sectionName)
            .Get<TSettings>(options =>
            {
                options.BindNonPublicProperties = false;
                options.ErrorOnUnknownConfiguration = true;
            });

        if (settings is null)
        {
            throw new InvalidOperationException(
                $"Unable to find section in configuration: {sectionName}");
        }

        if (!settings.IsValid())
        {
            throw new InvalidOperationException(
                $"Some values in settings {typeof(TSettings).Name} are invalid.");
        }

        return settings;
    }

    private string GetSectionName(Type settingsType)
    {
        _sectionNamesLock.EnterReadLock();

        try
        {
            if (_sectionNames.TryGetValue(settingsType, out var result))
            {
                return result;
            }
        }
        finally
        {
            _sectionNamesLock.ExitReadLock();
        }

        _sectionNamesLock.EnterWriteLock();

        try
        {
            if (_sectionNames.TryGetValue(settingsType, out var sectionName))
            {
                return sectionName;
            }

            sectionName = GetSectionNameUsingReflection(settingsType);

            _sectionNames.Add(settingsType, sectionName);

            return sectionName;
        }
        finally
        {
            _sectionNamesLock.ExitWriteLock();
        }
    }

    private static void AddEnvironmentJsonFile(
        IConfigurationBuilder configurationBuilder,
        string environmentJsonFileName)
    {
        var indexOfOpening = environmentJsonFileName.IndexOf('{');
        var indexOfClosing = environmentJsonFileName.IndexOf('}');

        if (indexOfOpening == -1 || indexOfClosing == -1 || indexOfClosing < indexOfOpening)
        {
            throw new FormatException(
                $"Specified environment json file name is invalid: '{environmentJsonFileName}'.");
        }

        var environmentVariableName = environmentJsonFileName
            .Substring(indexOfOpening + 1, indexOfClosing - indexOfOpening - 1);

        var environmentVariable = Environment.GetEnvironmentVariable(environmentVariableName);

        if (!string.IsNullOrWhiteSpace(environmentVariable))
        {
            var fileName = environmentJsonFileName.Replace($"{{{environmentVariableName}}}", environmentVariable);

            configurationBuilder.AddJsonFile(
                fileName,
                optional: true,
                reloadOnChange: false);
        }
    }

    private static string GetSectionNameUsingReflection(Type settingsType)
    {
        var settingsGenericType = settingsType
            .GetInterfaces()
            .Where(i => i.IsGenericType)
            .Single(i => i.GetGenericTypeDefinition() == typeof(ISettings<>));

        var descriptorType = settingsGenericType.GetGenericArguments()[0];
        var descriptorInstance = Activator.CreateInstance(descriptorType);

        if (descriptorInstance is null)
        {
            throw new InvalidOperationException(
                $"Cannot create settings descriptor instance from type: {descriptorType.Name}");
        }

        if (descriptorInstance is not ISettingsDescriptor descriptorTyped)
        {
            throw new InvalidOperationException(
                "Invalid descriptor type.");
        }

        return descriptorTyped.Section;
    }
}
