using Holidays.Core.Eventing;
using Holidays.Core.OfferModel;

namespace Holidays.Postgres.EventHandlers;

public class OfferPriceChangedPostgresEventHandler : IEventHandler<OfferPriceChanged>
{
    private readonly PostgresConnectionFactory _connectionFactory;

    public OfferPriceChangedPostgresEventHandler(PostgresConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task Handle(OfferPriceChanged @event, CancellationToken cancellationToken)
    {
        await using var connection = await _connectionFactory.CreateConnection(cancellationToken);
        await using var transaction = await connection.BeginTransactionAsync(cancellationToken);
        
        await using var offersRepository = new OffersPostgresRepository(connection, transaction);
        await using var offerChangesRepository = new OfferEventLogPostgresRepository(connection, transaction);

        try
        {
            await offersRepository.ModifyPrice(@event.OfferId, @event.CurrentPrice);
            await offerChangesRepository.Add(@event);

            await transaction.CommitAsync(cancellationToken);
        }
        catch
        {
            await transaction.RollbackAsync(CancellationToken.None);
            throw;
        }
    }
}
