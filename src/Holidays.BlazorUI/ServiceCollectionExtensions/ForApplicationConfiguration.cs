using Holidays.Configuration;

namespace Holidays.BlazorUI.ServiceCollectionExtensions;

internal static class ForApplicationConfiguration
{
    public static IServiceCollection AddApplicationConfiguration(
        this IServiceCollection services,
        IConfigurationRoot configurationRoot)
    {
        var configuration = new ApplicationConfiguration(configurationRoot);

        return services.AddSingleton(configuration);
    }
}
