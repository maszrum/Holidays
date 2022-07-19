using Holidays.Configuration;
using Holidays.Postgres;

namespace Holidays.BlazorUI.ServiceCollectionExtensions;

internal static class ForPostgres
{
    public static IServiceCollection AddPostgres(this IServiceCollection services)
    {
        return services.AddSingleton(serviceProvider =>
        {
            var configuration = serviceProvider.GetRequiredService<ApplicationConfiguration>();
            var postgresSettings = configuration.Get<PostgresSettings>();

            return new PostgresConnectionFactory(postgresSettings);
        });
    }
}
