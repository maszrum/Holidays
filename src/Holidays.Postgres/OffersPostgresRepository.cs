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

    public Task<Offers> GetAll() =>
        GetAllOffers(getRemoved: false);

    public Task<Offers> GetAllByWebsiteName(string websiteName) =>
        GetAllOffersByWebsiteName(websiteName, getRemoved: false);

    public Task<Offers> GetAllRemovedByWebsiteName(string websiteName) =>
        GetAllOffersByWebsiteName(websiteName, getRemoved: true);

    public async Task<Maybe<Offer>> Get(Guid offerId)
    {
        var record = await Connection.QuerySingleOrDefaultAsync<OfferDbRecord>(
            sql: "SELECT * FROM holidays.offer WHERE id = @Id",
            param: new { Id = offerId },
            transaction: Transaction);

        if (record is null)
        {
            return Maybe.None<Offer>();
        }

        var offer = _converter.ConvertToObject(record);

        return Maybe.Some(offer);
    }

    public async Task<Maybe<DateOnly>> GetLastDepartureDate(string websiteName)
    {
        const string sql =
            "SELECT departure_date " +
            "FROM holidays.offer " +
            "WHERE website_name = @WebsiteName " +
            "ORDER BY departure_date DESC " +
            "LIMIT 1";

        var departureDateDay = await Connection.QuerySingleOrDefaultAsync<int?>(
            sql: sql,
            param: new { WebsiteName = websiteName },
            transaction: Transaction);

        return departureDateDay.HasValue
            ? Maybe.Some(DateOnly.FromDayNumber(departureDateDay.Value))
            : Maybe.None<DateOnly>();
    }

    public async Task Add(Offer offer)
    {
        var (alreadyExists, isRemoved) = await OfferExists(offer.Id);

        string sql;
        object param;

        if (alreadyExists && !isRemoved)
        {
            throw new InvalidOperationException(
                $"An error occured on adding offer to database: '{offer.Id}' already exists.");
        }

        if (alreadyExists && isRemoved)
        {
            sql = "UPDATE holidays.offer SET is_removed = FALSE, price = @Price WHERE id = @Id";
            param = new { offer.Id, offer.Price };
        }
        else
        {
            var record = _converter.ConvertToRecord(offer, isRemoved: false);

            sql =
                "INSERT INTO holidays.offer " +
                "(id, hotel, destination_country, detailed_destination, departure_date, days, city_of_departure, price, details_url, website_name, is_removed) " +
                "VALUES " +
                "(@Id, @Hotel, @DestinationCountry, @DetailedDestination, @DepartureDate, @Days, @CityOfDeparture, @Price, @DetailsUrl, @WebsiteName, @IsRemoved)";

            param = record;
        }

        var rowsAffected = await Connection.ExecuteAsync(
            sql: sql,
            param: param,
            transaction: Transaction);

        if (rowsAffected != 1)
        {
            throw new InvalidOperationException(
                $"An error occured on adding offer to database: '{offer.Id}'.");
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
                $"An error occured on updating offer price in database: '{offerId}'.");
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
                $"An error occured on deleting offer from database: '{offerId}'.");
        }
    }

    public Task<int> Count() => CountOffers(countRemoved: false);

    public Task<int> CountRemoved() => CountOffers(countRemoved: true);

    private async Task<Offers> GetAllOffers(bool getRemoved)
    {
        var records = await Connection.QueryAsync<OfferDbRecord>(
            sql: $"SELECT * FROM holidays.offer WHERE is_removed = @IsRemoved",
            param: new { IsRemoved = getRemoved },
            transaction: Transaction);

        var offers = records.Select(_converter.ConvertToObject);

        return new Offers(offers);
    }

    private async Task<Offers> GetAllOffersByWebsiteName(string websiteName, bool getRemoved)
    {
        var records = await Connection.QueryAsync<OfferDbRecord>(
            sql: $"SELECT * FROM holidays.offer WHERE website_name = @WebsiteName AND is_removed = @IsRemoved",
            param: new { WebsiteName = websiteName, IsRemoved = getRemoved },
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
