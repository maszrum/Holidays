using Holidays.Core.Events.OfferModel;
using Holidays.Core.OfferModel;

namespace Holidays.BlazorUI.ViewModels;

public class OfferViewModel
{
    private OfferViewModel(
        Guid id,
        string hotel,
        string destinationCountry,
        string detailedDestination,
        DateOnly departureDate,
        int days,
        string cityOfDeparture,
        int price,
        string detailsUrl,
        string websiteName)
    {
        Id = id;
        Hotel = hotel;
        DestinationCountry = destinationCountry;
        DetailedDestination = detailedDestination;
        DepartureDate = departureDate;
        Days = days;
        CityOfDeparture = cityOfDeparture;
        Price = price;
        DetailsUrl = detailsUrl;
        WebsiteName = websiteName;
    }

    public Guid Id { get; init; }

    public string Hotel { get; init; }

    public string DestinationCountry { get; init; }

    public string DetailedDestination { get; init; }

    public DateOnly DepartureDate { get; init; }

    public int Days { get; init; }

    public string CityOfDeparture { get; init; }

    public int Price { get; private set; }

    public string DetailsUrl { get; init; }

    public string WebsiteName { get; init; }

    public int? PreviousPrice { get; private set; }

    public bool IsNew { get; private set; }

    public bool IsRemoved { get; private set; }

    public void Acknowledge()
    {
        PreviousPrice = default;
        IsNew = false;
    }

    public void ChangePrice(int newPrice)
    {
        PreviousPrice = Price;
        Price = newPrice;
    }

    public void MarkAsRemoved()
    {
        IsRemoved = true;
    }

    private void MarkAsNew()
    {
        IsNew = true;
    }

    public static OfferViewModel FromEvent(OfferStartedTracking @event)
    {
        var offerData = @event.OfferData!;

        return new OfferViewModel(
            @event.OfferId,
            offerData.Hotel,
            offerData.DestinationCountry,
            offerData.DetailedDestination,
            offerData.DepartureDate,
            offerData.Days,
            offerData.CityOfDeparture,
            offerData.Price,
            offerData.DetailsUrl,
            offerData.WebsiteName);
    }

    public static OfferViewModel FromEvent(OfferAdded @event)
    {
        var offerData = @event.OfferData!;

        var viewModel = new OfferViewModel(
            @event.OfferId,
            offerData.Hotel,
            offerData.DestinationCountry,
            offerData.DetailedDestination,
            offerData.DepartureDate,
            offerData.Days,
            offerData.CityOfDeparture,
            offerData.Price,
            offerData.DetailsUrl,
            offerData.WebsiteName);

        viewModel.MarkAsNew();

        return viewModel;
    }

    public static OfferViewModel FromOffer(Offer offer)
    {
        return new OfferViewModel(
            offer.Id,
            offer.Hotel,
            offer.DestinationCountry,
            offer.DetailedDestination,
            offer.DepartureDate,
            offer.Days,
            offer.CityOfDeparture,
            offer.Price,
            offer.DetailsUrl,
            offer.WebsiteName);
    }
}
