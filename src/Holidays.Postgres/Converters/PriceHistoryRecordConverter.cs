using Holidays.Core.OfferModel;
using Holidays.Postgres.DbRecords;

namespace Holidays.Postgres.Converters;

internal class PriceHistoryRecordConverter
{
    public PriceHistory ToObject(Guid offerId, IEnumerable<PriceHistoryDbRecord> records)
    {
        var pricesAndTimestamps = records.ToDictionary(
            r => r.PriceTimestamp,
            r => r.Price);

        return new PriceHistory(offerId, pricesAndTimestamps);
    }
}
