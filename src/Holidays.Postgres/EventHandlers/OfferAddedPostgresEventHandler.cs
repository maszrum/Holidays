using Holidays.Core.Eventing;
using Holidays.Core.OfferModel;

namespace Holidays.Postgres.EventHandlers;

public class OfferAddedPostgresEventHandler : IEventHandler<OfferAdded>
{
    private readonly PostgresConnectionFactory _connectionFactory;

    public OfferAddedPostgresEventHandler(PostgresConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task Handle(OfferAdded @event, Func<Task> next, CancellationToken cancellationToken)
    {
        if (@event.Offer is null)
        {
            throw new InvalidOperationException(
                "Received event without offer data.");
        }

        await using var connection = await _connectionFactory.CreateConnection(cancellationToken);
        await using var transaction = await connection.BeginTransactionAsync(cancellationToken);

        await using var offersRepository = new OffersPostgresRepository(connection, transaction);
        await using var offerChangesRepository = new OfferEventLogPostgresRepository(connection, transaction);
        await using var priceHistoryRepository = new PriceHistoryPostgresRepository(connection, transaction);

        try
        {
            await offersRepository.Add(@event.Offer);
            await offerChangesRepository.Add(@event);
            await priceHistoryRepository.Add(@event.Offer.Id, @event.Timestamp, @event.Offer.Price);

            await next();

            await transaction.CommitAsync(cancellationToken);
        }
        catch
        {
            await transaction.RollbackAsync(CancellationToken.None);
            throw;
        }
    }
}
