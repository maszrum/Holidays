using Dapper;
using Holidays.Core.InfrastructureInterfaces;
using Holidays.Core.Monads;
using Holidays.Core.OfferModel;
using Holidays.Postgres.Converters;
using Holidays.Postgres.DbRecords;
using Npgsql;

namespace Holidays.Postgres;

public sealed class OffersPostgresRepository : PostgresRepositoryBase, IOffersRepository
{
    private readonly OfferRecordConverter _converter = new();
    
    public OffersPostgresRepository(NpgsqlConnection connection) 
        : base(connection)
    {
    }

    public OffersPostgresRepository(NpgsqlConnection connection, NpgsqlTransaction transaction) 
        : base(connection, transaction)
    {
    }

    public Task<Offers> GetAll() => GetAllOffers(getRemoved: false);

    public Task<Offers> GetAllRemoved() => GetAllOffers(getRemoved: true);

    public async Task<Maybe<Offer>> Get(Guid offerId)
    {
        var record = await Connection.QuerySingleOrDefaultAsync<OfferDbRecord>(
            sql: "SELECT * FROM holidays.offer WHERE id = @Id",
            param: new { Id = offerId },
            transaction: Transaction);

        if (record is null)
        {
            return Maybe.Null<Offer>();
        }

        var offer = _converter.ConvertToObject(record);

        return offer;
    }
    
    public async Task Add(Offer offer)
    {
        var (alreadyExists, isRemoved) = await OfferExists(offer.Id);

        string sql;
        object param;

        if (alreadyExists && !isRemoved)
        {
            throw new InvalidOperationException(
                "An error occured on adding offer to database: already exists.");
        }
        
        if (alreadyExists && isRemoved)
        {
            sql = "UPDATE holidays.offer SET is_removed = FALSE WHERE id = @Id";
            param = new { offer.Id };
        }
        else
        {
            var record = _converter.ConvertToRecord(offer, isRemoved: false);
        
            sql = 
                "INSERT INTO holidays.offer " +
                "(id, hotel, destination, departure_date, days, city_of_departure, price, details_url, is_removed) " +
                "VALUES " +
                "(@Id, @Hotel, @Destination, @DepartureDate, @Days, @CityOfDeparture, @Price, @DetailsUrl, @IsRemoved)";

            param = record;
        }
        
        var rowsAffected = await Connection.ExecuteAsync(
            sql: sql, 
            param: param, 
            transaction: Transaction);

        if (rowsAffected != 1)
        {
            throw new InvalidOperationException(
                "An error occured on adding offer to database.");
        }
    }

    public async Task ModifyPrice(Guid offerId, int newPrice)
    {
        var rowsAffected = await Connection.ExecuteAsync(
            sql: "UPDATE holidays.offer SET price = @Price WHERE id = @Id",
            param: new { Id = offerId, Price = newPrice },
            transaction: Transaction);

        if (rowsAffected != 1)
        {
            throw new InvalidOperationException(
                "An error occured on updating offer price in database.");
        }
    }
    
    public async Task Remove(Guid offerId)
    {
        var rowsAffected = await Connection.ExecuteAsync(
            sql: "UPDATE holidays.offer SET is_removed = TRUE WHERE id = @Id",
            param: new { Id = offerId },
            transaction: Transaction);

        if (rowsAffected != 1)
        {
            throw new InvalidOperationException(
                "An error occured on deleting offer from database.");
        }
    }

    public Task<int> Count() => CountOffers(countRemoved: false);

    public Task<int> CountRemoved() => CountOffers(countRemoved: true);

    private async Task<Offers> GetAllOffers(bool getRemoved)
    {
        var trueFalse = getRemoved ? "TRUE" : "FALSE";
        
        var records = await Connection.QueryAsync<OfferDbRecord>(
            sql: $"SELECT * FROM holidays.offer WHERE is_removed = {trueFalse}",
            transaction: Transaction);

        var offers = records.Select(_converter.ConvertToObject);

        return new Offers(offers);
    }

    private async Task<(bool Exists, bool IsRemoved)> OfferExists(Guid offerId)
    {
        var record = await Connection.QuerySingleOrDefaultAsync<OfferDbRecord>(
            sql: "SELECT * FROM holidays.offer WHERE id = @Id",
            param: new { Id = offerId },
            transaction: Transaction);

        return record is null 
            ? (false, false) 
            : (true, record.IsRemoved);
    }

    private async Task<int> CountOffers(bool countRemoved)
    {
        var trueFalse = countRemoved ? "TRUE" : "FALSE";

        var count = await Connection.ExecuteScalarAsync<int>(
            sql: $"SELECT COUNT(1) FROM holidays.offer WHERE is_removed = {trueFalse}", 
            transaction: Transaction);

        return count;
    }
}
