namespace Holidays.Postgres.DbRecords;

public class PriceHistoryDbRecord
{
    // ReSharper disable once UnusedMember.Local
    private PriceHistoryDbRecord()
    {
    }

    public PriceHistoryDbRecord(
        Guid id,
        DateTime priceTimestamp,
        Guid offerId,
        int price)
    {
        Id = id;
        PriceTimestamp = priceTimestamp;
        OfferId = offerId;
        Price = price;
    }

    public Guid Id { get; init; }

    public DateTime PriceTimestamp { get; }

    public Guid OfferId { get; init; }

    public int Price { get; init; }
}
