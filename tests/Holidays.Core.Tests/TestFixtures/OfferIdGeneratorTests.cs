using Holidays.Core.OfferModel;
using NUnit.Framework;

namespace Holidays.Core.Tests.TestFixtures;

[TestFixture]
public class OfferIdGeneratorTests
{
    [Test]
    public void same_offers_should_have_same_id()
    {
        var offer = new Offer(
            hotel: "hotel", 
            destination: "destination", 
            departureDate: DateOnly.FromDayNumber(2), 
            days: 3, 
            cityOfDeparture: "city", 
            price: 1200, 
            detailsUrl: "url");

        var offerId = offer.Id;
        
        var sameOffer = new Offer(
            hotel: "hotel", 
            destination: "destination", 
            departureDate: DateOnly.FromDayNumber(2), 
            days: 3, 
            cityOfDeparture: "city", 
            price: 1200, 
            detailsUrl: "url");

        var sameOfferId = sameOffer.Id;
        
        Assert.That(sameOfferId, Is.EqualTo(offerId));
    }
    
    [Test]
    public void same_offers_with_different_prices_should_have_same_id()
    {
        var offer = new Offer(
            hotel: "hotel", 
            destination: "destination", 
            departureDate: DateOnly.FromDayNumber(2), 
            days: 3, 
            cityOfDeparture: "city", 
            price: 1200, 
            detailsUrl: "url");

        var offerId = offer.Id;

        var offerWithDifferentPrice = new Offer(
            hotel: "hotel", 
            destination: "destination", 
            departureDate: DateOnly.FromDayNumber(2), 
            days: 3, 
            cityOfDeparture: "city", 
            price: 1400, 
            detailsUrl: "url");

        var offerIdWithDifferentPrice = offerWithDifferentPrice.Id;
        
        Assert.That(offerIdWithDifferentPrice, Is.EqualTo(offerId));
    }
    
    [Test]
    public void same_offers_with_different_hotel_should_have_different_id()
    {
        var offer = new Offer(
            hotel: "hotel", 
            destination: "destination", 
            departureDate: DateOnly.FromDayNumber(2), 
            days: 3, 
            cityOfDeparture: "city", 
            price: 1200, 
            detailsUrl: "url");

        var offerId = offer.Id;

        var offerWithDifferentHotel = new Offer(
            hotel: "different hotel", 
            destination: "destination", 
            departureDate: DateOnly.FromDayNumber(2), 
            days: 3, 
            cityOfDeparture: "city", 
            price: 1200, 
            detailsUrl: "url");

        var offerIdWithDifferentHotel = offerWithDifferentHotel.Id;
        
        Assert.That(offerIdWithDifferentHotel, Is.Not.EqualTo(offerId));
    }
    
    [Test]
    public void same_offers_with_different_destination_should_have_different_id()
    {
        var offer = new Offer(
            hotel: "hotel", 
            destination: "destination", 
            departureDate: DateOnly.FromDayNumber(2), 
            days: 3, 
            cityOfDeparture: "city", 
            price: 1200, 
            detailsUrl: "url");

        var offerId = offer.Id;

        var offerWithDifferentDestination = new Offer(
            hotel: "hotel", 
            destination: "different destination", 
            departureDate: DateOnly.FromDayNumber(2), 
            days: 3, 
            cityOfDeparture: "city", 
            price: 1200, 
            detailsUrl: "url");

        var offerIdWithDifferentDestination = offerWithDifferentDestination.Id;
        
        Assert.That(offerIdWithDifferentDestination, Is.Not.EqualTo(offerId));
    }
    
    [Test]
    public void same_offers_with_different_departure_date_should_have_different_id()
    {
        var offer = new Offer(
            hotel: "hotel", 
            destination: "destination", 
            departureDate: DateOnly.FromDayNumber(2), 
            days: 3, 
            cityOfDeparture: "city", 
            price: 1200, 
            detailsUrl: "url");

        var offerId = offer.Id;

        var offerWithDifferentDepartureDate = new Offer(
            hotel: "hotel", 
            destination: "destination", 
            departureDate: DateOnly.FromDayNumber(3), 
            days: 3, 
            cityOfDeparture: "city", 
            price: 1200, 
            detailsUrl: "url");

        var offerIdWithDifferentDepartureDate = offerWithDifferentDepartureDate.Id;
        
        Assert.That(offerIdWithDifferentDepartureDate, Is.Not.EqualTo(offerId));
    }
    
    [Test]
    public void same_offers_with_different_days_should_have_different_id()
    {
        var offer = new Offer(
            hotel: "hotel", 
            destination: "destination", 
            departureDate: DateOnly.FromDayNumber(2), 
            days: 3, 
            cityOfDeparture: "city", 
            price: 1200, 
            detailsUrl: "url");

        var offerId = offer.Id;

        var offerWithDifferentDays = new Offer(
            hotel: "hotel", 
            destination: "destination", 
            departureDate: DateOnly.FromDayNumber(2), 
            days: 4, 
            cityOfDeparture: "city", 
            price: 1200, 
            detailsUrl: "url");

        var offerIdWithDifferentDays = offerWithDifferentDays.Id;
        
        Assert.That(offerIdWithDifferentDays, Is.Not.EqualTo(offerId));
    }
    
    [Test]
    public void same_offers_with_different_city_of_departure_should_have_different_id()
    {
        var offer = new Offer(
            hotel: "hotel", 
            destination: "destination", 
            departureDate: DateOnly.FromDayNumber(2), 
            days: 3, 
            cityOfDeparture: "city", 
            price: 1200, 
            detailsUrl: "url");

        var offerId = offer.Id;

        var offerWithDifferentCityOfDeparture = new Offer(
            hotel: "hotel", 
            destination: "destination", 
            departureDate: DateOnly.FromDayNumber(2), 
            days: 3, 
            cityOfDeparture: "different city", 
            price: 1200, 
            detailsUrl: "url");

        var offerIdWithDifferentCityOfDeparture = offerWithDifferentCityOfDeparture.Id;
        
        Assert.That(offerIdWithDifferentCityOfDeparture, Is.Not.EqualTo(offerId));
    }
}
