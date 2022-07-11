using Holidays.Core.OfferModel;
using Holidays.Postgres.DbRecords;

namespace Holidays.Postgres.Converters;

internal class OfferPriceChangedConverter : EventConverterBase<OfferPriceChanged>
{
    protected override Guid GetOfferId(OfferPriceChanged @event) => 
        @event.OfferId;

    protected override string GetEventParams(OfferPriceChanged @event) => 
        $"{@event.PreviousPrice},{@event.CurrentPrice}";

    protected override OfferPriceChanged ToObject(OfferEventLogRecord record)
    {
        var paramsParts = record.Params.Split(',');

        if (paramsParts.Length != 2)
        {
            throw new InvalidOperationException(
                $"Cannot convert record to {nameof(OfferPriceChanged)}: invalid params.");
        }

        var previousPrice = int.Parse(paramsParts[0]);
        var currentPrice = int.Parse(paramsParts[1]);

        return new OfferPriceChanged(record.OfferId, currentPrice, previousPrice);
    }
}
