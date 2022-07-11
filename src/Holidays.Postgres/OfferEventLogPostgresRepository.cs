using Dapper;
using Holidays.Core.Eventing;
using Holidays.Core.InfrastructureInterfaces;
using Holidays.Core.OfferModel;
using Holidays.Postgres.Converters;
using Holidays.Postgres.DbRecords;
using Npgsql;

namespace Holidays.Postgres;

public sealed class OfferEventLogPostgresRepository : PostgresRepositoryBase, IOfferEventLogRepository
{
    private readonly EventRecordConverter _converter = new();
    
    public OfferEventLogPostgresRepository(NpgsqlConnection connection) 
        : base(connection)
    {
    }

    public OfferEventLogPostgresRepository(NpgsqlConnection connection, NpgsqlTransaction transaction) 
        : base(connection, transaction)
    {
    }
    
    public Task Add(OfferAdded @event)
    {
        var record = _converter.ConvertToRecord(@event);

        return AddEventLogRecord(record);
    }

    public Task Add(OfferPriceChanged @event)
    {
        var record = _converter.ConvertToRecord(@event);

        return AddEventLogRecord(record);
    }

    public Task Add(OfferRemoved @event)
    {
        var record = _converter.ConvertToRecord(@event);

        return AddEventLogRecord(record);
    }

    public async Task<int> Count()
    {
        var count = await Connection.ExecuteScalarAsync<int>(
            sql: "SELECT COUNT(1) FROM holidays.offer_event_log",
            transaction: Transaction);

        return count;
    }

    public async Task<IReadOnlyList<IEvent>> GetByOfferId(Guid offerId)
    {
        var records = await Connection.QueryAsync<OfferEventLogRecord>(
            sql: "SELECT * FROM holidays.offer_event_log WHERE offer_id = @Id",
            param: new { Id = offerId },
            transaction: Transaction);

        var events = records
            .Select(_converter.ConvertToObject)
            .ToArray();

        return events;
    }

    private async Task AddEventLogRecord(OfferEventLogRecord record)
    {
        var sql = 
            "INSERT INTO holidays.offer_event_log " +
            "(id, offer_id, event_type, params) " +
            "VALUES " +
            "(@Id, @OfferId, @EventType, @Params)";

        var rowsAffected = await Connection.ExecuteAsync(
            sql: sql,
            param: record,
            transaction: Transaction);

        if (rowsAffected != 1)
        {
            throw new InvalidOperationException(
                "An error occured on adding event log to database.");
        }
    }
}
