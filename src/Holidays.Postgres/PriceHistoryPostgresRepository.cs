using Dapper;
using Holidays.Core.InfrastructureInterfaces;
using Holidays.Core.Monads;
using Holidays.Core.OfferModel;
using Holidays.Postgres.Converters;
using Holidays.Postgres.DbRecords;
using Npgsql;

namespace Holidays.Postgres;

public class PriceHistoryPostgresRepository : PostgresRepositoryBase, IPriceHistoryRepository
{
    private readonly PriceHistoryRecordConverter _converter = new();

    public PriceHistoryPostgresRepository(NpgsqlConnection connection)
        : base(connection)
    {
    }

    public PriceHistoryPostgresRepository(NpgsqlConnection connection, NpgsqlTransaction transaction)
        : base(connection, transaction)
    {
    }

    public async Task<Maybe<PriceHistory>> Get(Guid offerId)
    {
        var records = await Connection.QueryAsync<PriceHistoryDbRecord>(
            sql: "SELECT * FROM holidays.price_history WHERE offer_id = @OfferId ORDER BY price_timestamp",
            param: new { offerId },
            transaction: Transaction);

        var recordsArray = records.ToArray();

        if (recordsArray.Length == 0)
        {
            return Maybe<PriceHistory>.None();
        }

        var priceHistory = _converter.ToObject(offerId, recordsArray);

        return Maybe.Some(priceHistory);
    }

    public async Task Add(Guid offerId, DateTime timestamp, int price)
    {
        var record = new PriceHistoryDbRecord(
            id: Guid.NewGuid(),
            priceTimestamp: timestamp,
            offerId: offerId,
            price: price);

        const string sql =
            "INSERT INTO holidays.price_history " +
            "(id, offer_id, price_timestamp, price) " +
            "VALUES " +
            "(@Id, @OfferId, @PriceTimestamp, @Price)";

        var rowsAffected = await Connection.ExecuteAsync(
            sql: sql,
            param: record,
            transaction: Transaction);

        if (rowsAffected != 1)
        {
            throw new InvalidOperationException(
                "An error occured on adding price history to database.");
        }
    }
}
