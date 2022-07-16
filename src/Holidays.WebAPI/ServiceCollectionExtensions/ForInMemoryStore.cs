using Holidays.InMemoryStore;
using Holidays.Postgres;

namespace Holidays.WebAPI.ServiceCollectionExtensions;

internal static class ForInMemoryStore
{
    public static IServiceCollection AddInMemoryStore(this IServiceCollection services)
    {
        return services.AddSingleton(serviceProvider =>
        {
            return InMemoryDatabase.CreateWithInitialStateFactory(async () =>
            {
                var postgresConnectionFactory = serviceProvider.GetRequiredService<PostgresConnectionFactory>();
                await using var postgresConnection = await postgresConnectionFactory.CreateConnection();

                var offersRepository = new OffersPostgresRepository(postgresConnection);
                var persistedOffers = await offersRepository.GetAll();

                return persistedOffers;
            });
        });
    }
}
