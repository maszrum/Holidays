using Holidays.Core.OfferModel;

namespace Holidays.Core.Algorithms.ChangesDetection;

public class OfferChangesDetector
{
    public IReadOnlyList<DetectedChange> DetectChanges(Offers previousState, Offers currentState)
    {
        var addedOffers = GetAddedOffers(previousState, currentState);
        var removedOffers = GetRemovedOffers(previousState, currentState);
        var priceChangedOffers = GetOffersWithChangedPrice(previousState, currentState);

        return addedOffers
            .Concat(removedOffers)
            .Concat(priceChangedOffers)
            .ToArray();
    }

    private static IEnumerable<DetectedChange> GetAddedOffers(Offers previousState, Offers currentState)
    {
        var addedOffers = currentState.Elements.Except(previousState.Elements);

        return addedOffers.Select(DetectedChange.OfferAdded);
    }

    private static IEnumerable<DetectedChange> GetRemovedOffers(Offers previousState, Offers currentState)
    {
        var removedOffers = previousState.Elements.Except(currentState.Elements);

        return removedOffers.Select(DetectedChange.OfferRemoved);
    }

    private static IEnumerable<DetectedChange> GetOffersWithChangedPrice(Offers previousState, Offers currentState)
    {
        var commonOffers = currentState.Elements.Intersect(previousState.Elements);
        
        foreach (var offer in commonOffers)
        {
            previousState.Elements.TryGetValue(offer, out var previousOffer);
            currentState.Elements.TryGetValue(offer, out var currentOffer);

            if (previousOffer.Price != currentOffer.Price)
            {
                var change = DetectedChange.PriceChanged(currentOffer, previousOffer);
                yield return change;
            }
        }
    }
}
