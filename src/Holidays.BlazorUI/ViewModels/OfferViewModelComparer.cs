namespace Holidays.BlazorUI.ViewModels;

// ReSharper disable EnforceIfStatementBraces

internal class OfferViewModelComparer : IComparer<OfferViewModel>
{
    public int Compare(OfferViewModel? x, OfferViewModel? y)
    {
        if (ReferenceEquals(x, y)) return 0;
        if (ReferenceEquals(null, y)) return 1;
        if (ReferenceEquals(null, x)) return -1;

        if (x.IsNew != y.IsNew)
        {
            return x.IsNew ? -1 : 1;
        }

        if (x.PreviousPrice.HasValue != y.PreviousPrice.HasValue)
        {
            return x.PreviousPrice.HasValue ? -1 : 1;
        }

        if (x.IsRemoved != y.IsRemoved)
        {
            return x.IsRemoved ? -1 : 1;
        }

        var priceComparision = x.Price.CompareTo(y.Price);
        if (priceComparision != 0)
        {
            return priceComparision;
        }

        var departureDateComparision = x.DepartureDate.CompareTo(y.DepartureDate);
        if (departureDateComparision != 0)
        {
            return departureDateComparision;
        }

        return x.Days.CompareTo(y.Days);
    }
}
