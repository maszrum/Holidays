using Holidays.Core.OfferModel;

namespace Holidays.Core.Algorithms.ChangesDetection;

public class DetectedChange
{
    private readonly Offer? _offerBeforeChange;

    private DetectedChange(OfferChangeType changeType, Offer offer, Offer? offerBeforeChange)
    {
        ChangeType = changeType;
        Offer = offer;
        _offerBeforeChange = offerBeforeChange;
    }

    public OfferChangeType ChangeType { get; }

    public Offer OfferBeforeChange
    {
        get
        {
            if (_offerBeforeChange is null)
            {
                throw new InvalidOperationException(
                    "Change does not contain information about previous state.");
            }

            return _offerBeforeChange;
        }
    }

    public Offer Offer { get; }

    public static DetectedChange PriceChanged(Offer offer, Offer offerBeforeChange) =>
        new(OfferChangeType.PriceChanged, offer, offerBeforeChange);

    public static DetectedChange OfferRemoved(Offer offer) =>
        new(OfferChangeType.OfferRemoved, offer, default);

    public static DetectedChange OfferAdded(Offer offer) =>
        new(OfferChangeType.OfferAdded, offer, default);
}
