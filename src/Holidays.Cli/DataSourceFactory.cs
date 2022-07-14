using System.Reflection;
using Holidays.Core.InfrastructureInterfaces;
using Holidays.Selenium;

namespace Holidays.Cli;

internal class DataSourceFactory
{
    private readonly WebDriverFactory _webDriverFactory;

    public DataSourceFactory(WebDriverFactory webDriverFactory)
    {
        _webDriverFactory = webDriverFactory;
    }

    public IOffersDataSource Get(string name)
    {
        var expectedDllName = $"{nameof(Holidays)}.{nameof(DataSource)}.{name}.dll";
        
        var currentAssemblyFileName = Assembly.GetExecutingAssembly().Location;
        var location = Path.GetDirectoryName(currentAssemblyFileName);

        if (string.IsNullOrWhiteSpace(location))
        {
            throw new InvalidOperationException(
                "Cannot get current assembly location.");
        }

        var expectedAssemblyPath = Path.Combine(location, expectedDllName);

        var assembly = Assembly.LoadFile(expectedAssemblyPath);

        var dataSourceTypes = assembly
            .GetTypes()
            .Where(t => !t.IsAbstract && typeof(IOffersDataSource).IsAssignableFrom(t))
            .ToArray();

        if (dataSourceTypes.Length == 0)
        {
            throw new InvalidOperationException(
                $"Specified data source was not found. " +
                $"Cannot find class that implements interface {nameof(IOffersDataSource)}.");
        }

        if (dataSourceTypes.Length > 1)
        {
            throw new InvalidOperationException(
                $"An error occured when loading data source. " +
                $"There is more than one class implementing {nameof(IOffersDataSource)} interface.");
        }

        var dataSource = Activator.CreateInstance(dataSourceTypes[0], _webDriverFactory);

        if (dataSource is not IOffersDataSource dataSourceTyped)
        {
            throw new InvalidOperationException(
                $"Cannot instantiate object of type {dataSourceTypes[0].Name} " +
                $"with one argument of type {nameof(WebDriverFactory)}.");
        }

        return dataSourceTyped;
    }
}
