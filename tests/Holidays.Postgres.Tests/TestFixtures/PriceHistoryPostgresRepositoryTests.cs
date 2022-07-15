using Holidays.Core.OfferModel;
using NUnit.Framework;

namespace Holidays.Postgres.Tests.TestFixtures;

[TestFixture]
public class PriceHistoryPostgresRepositoryTests : DatabaseTestsBase
{
    [Test]
    public async Task get_price_history_should_return_empty_list()
    {
        var getPriceHistory = await DoWithTransactionAndRollback(async (connection, transaction) =>
        {
            var repository = new PriceHistoryPostgresRepository(connection, transaction);
            var priceHistory = await repository.Get(Guid.NewGuid());
            return priceHistory;
        });

        Assert.That(getPriceHistory.Prices, Has.Count.EqualTo(0));
    }

    [Test]
    public async Task get_price_history_should_return_correct_values_correctly_ordered()
    {
        var offer = new Offer("hotel", "destination", DateOnly.FromDayNumber(3), 4, "city", 2200, "url", "website");
        var dateTime = new DateTime(2022, 7, 11, 18, 52, 12);

        var getPriceHistory = await DoWithTransactionAndRollback(async (connection, transaction) =>
        {
            var offersRepository = new OffersPostgresRepository(connection, transaction);
            var priceHistoryRepository = new PriceHistoryPostgresRepository(connection, transaction);

            await offersRepository.Add(offer);
            await priceHistoryRepository.Add(offer.Id, dateTime.AddSeconds(1), 1200);
            await priceHistoryRepository.Add(offer.Id, dateTime.AddSeconds(-1), 1300);
            await priceHistoryRepository.Add(offer.Id, dateTime.AddSeconds(2), 1400);

            var priceHistory = await priceHistoryRepository.Get(offer.Id);
            return priceHistory;
        });

        Assert.That(getPriceHistory.Prices, Has.Count.EqualTo(3));

        CollectionAssert.AreEqual(
            new[] { 1300, 1200, 1400 },
            getPriceHistory.Prices.Values);
    }
}
