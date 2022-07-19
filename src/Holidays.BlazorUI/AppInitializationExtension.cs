using Holidays.BlazorUI.Services;
using Holidays.Eventing;
using Holidays.InMemoryStore;

namespace Holidays.BlazorUI;

internal static class AppInitializationExtension
{
    public static async Task InitializeServices(this IServiceProvider serviceProvider)
    {
        await serviceProvider.GetRequiredService<IEventBus>().Initialize();

        var inMemoryDatabase = serviceProvider.GetRequiredService<InMemoryDatabase>();
        await inMemoryDatabase.Initialize();

        var offersService = serviceProvider.GetRequiredService<OffersService>();
        var offersRepository = new OffersInMemoryRepository(inMemoryDatabase);
        await offersService.Initialize(offersRepository);
    }
}
