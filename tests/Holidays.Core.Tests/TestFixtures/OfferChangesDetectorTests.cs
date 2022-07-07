using Holidays.Core.Algorithms.ChangesDetection;
using Holidays.Core.OfferModel;
using NUnit.Framework;

namespace Holidays.Core.Tests.TestFixtures;

[TestFixture]
public class OfferChangesDetectorTests
{
    [Test]
    public void given_no_offers_and_then_there_should_be_no_changes_detected()
    {
        var previousState = new Offers(Enumerable.Empty<Offer>());
        var currentState = new Offers(Enumerable.Empty<Offer>());

        var changes = new OfferChangesDetector().DetectChanges(previousState, currentState);
        
        Assert.That(changes, Has.Count.Zero);
    }
    
    [Test]
    public void given_three_offers_and_then_there_should_be_no_changes_detected()
    {
        var previousState = Create.Offers(
            new Offer("hotel a", "destination a", DateOnly.FromDayNumber(1), 8, "city a", 1, "url a"),
            new Offer("hotel a", "destination a", DateOnly.FromDayNumber(2), 8, "city a", 2, "url b"),
            new Offer("hotel b", "destination b", DateOnly.FromDayNumber(1), 8, "city a", 1, "url c"));
        
        var currentState = Create.Offers(
            new Offer("hotel a", "destination a", DateOnly.FromDayNumber(1), 8, "city a", 1, "url a"),
            new Offer("hotel a", "destination a", DateOnly.FromDayNumber(2), 8, "city a", 2, "url b"),
            new Offer("hotel b", "destination b", DateOnly.FromDayNumber(1), 8, "city a", 1, "url c"));

        var changes = new OfferChangesDetector().DetectChanges(previousState, currentState);
        
        Assert.That(changes, Has.Count.Zero);
    }
    
    [Test]
    public void given_three_offers_and_then_there_should_be_one_price_change_detected()
    {
        var previousState = Create.Offers(
            new Offer("hotel a", "destination a", DateOnly.FromDayNumber(1), 8, "city a", 1, "url a"),
            new Offer("hotel a", "destination a", DateOnly.FromDayNumber(2), 8, "city a", 2, "url b"),
            new Offer("hotel b", "destination b", DateOnly.FromDayNumber(1), 8, "city a", 1, "url c"));
        
        var currentState = Create.Offers(
            new Offer("hotel a", "destination a", DateOnly.FromDayNumber(1), 8, "city a", 1, "url a"),
            new Offer("hotel a", "destination a", DateOnly.FromDayNumber(2), 8, "city a", 2000, "url b"),
            new Offer("hotel b", "destination b", DateOnly.FromDayNumber(1), 8, "city a", 1, "url c"));

        var changes = new OfferChangesDetector().DetectChanges(previousState, currentState);
        
        Assert.That(changes, Has.Count.EqualTo(1));
        var change = changes[0];
        Assert.That(change.ChangeType, Is.EqualTo(OfferChangeType.PriceChanged));
        Assert.That(change.Offer.Price, Is.EqualTo(2000));
        Assert.That(change.OfferBeforeChange.Price, Is.EqualTo(2));
    }
    
    [Test]
    public void given_three_offers_and_then_there_should_be_one_offer_removed_detected()
    {
        var previousState = Create.Offers(
            new Offer("hotel a", "destination a", DateOnly.FromDayNumber(1), 8, "city a", 1, "url a"),
            new Offer("hotel a", "destination a", DateOnly.FromDayNumber(2), 8, "city a", 2, "url b"),
            new Offer("hotel b", "destination b", DateOnly.FromDayNumber(1), 8, "city a", 1, "url c"));
        
        var currentState = Create.Offers(
            new Offer("hotel a", "destination a", DateOnly.FromDayNumber(1), 8, "city a", 1, "url a"),
            new Offer("hotel b", "destination b", DateOnly.FromDayNumber(1), 8, "city a", 1, "url c"));

        var changes = new OfferChangesDetector().DetectChanges(previousState, currentState);
        
        Assert.That(changes, Has.Count.EqualTo(1));
        var change = changes[0];
        
        Assert.Multiple(() =>
        {
            Assert.That(change.ChangeType, Is.EqualTo(OfferChangeType.OfferRemoved));
            Assert.That(change.Offer.Hotel, Is.EqualTo("hotel a"));
            Assert.That(change.Offer.Destination, Is.EqualTo("destination a"));
            Assert.That(change.Offer.DepartureDate, Is.EqualTo(DateOnly.FromDayNumber(2)));
            Assert.That(change.Offer.Days, Is.EqualTo(8));
            Assert.That(change.Offer.CityOfDeparture, Is.EqualTo("city a"));
            Assert.That(change.Offer.Price, Is.EqualTo(2));
            Assert.That(change.Offer.DetailsUrl, Is.EqualTo("url b"));
        });
    }
    
    [Test]
    public void given_three_offers_and_then_there_should_be_one_offer_added_detected()
    {
        var previousState = Create.Offers(
            new Offer("hotel a", "destination a", DateOnly.FromDayNumber(1), 8, "city a", 1, "url a"),
            new Offer("hotel b", "destination b", DateOnly.FromDayNumber(1), 8, "city a", 1, "url c"));
        
        var currentState = Create.Offers(
            new Offer("hotel a", "destination a", DateOnly.FromDayNumber(1), 8, "city a", 1, "url a"),
            new Offer("hotel a", "destination a", DateOnly.FromDayNumber(2), 8, "city a", 2, "url b"),
            new Offer("hotel b", "destination b", DateOnly.FromDayNumber(1), 8, "city a", 1, "url c"));

        var changes = new OfferChangesDetector().DetectChanges(previousState, currentState);
        
        Assert.That(changes, Has.Count.EqualTo(1));
        var change = changes[0];
        
        Assert.Multiple(() =>
        {
            Assert.That(change.ChangeType, Is.EqualTo(OfferChangeType.OfferAdded));
            Assert.That(change.Offer.Hotel, Is.EqualTo("hotel a"));
            Assert.That(change.Offer.Destination, Is.EqualTo("destination a"));
            Assert.That(change.Offer.DepartureDate, Is.EqualTo(DateOnly.FromDayNumber(2)));
            Assert.That(change.Offer.Days, Is.EqualTo(8));
            Assert.That(change.Offer.CityOfDeparture, Is.EqualTo("city a"));
            Assert.That(change.Offer.Price, Is.EqualTo(2));
            Assert.That(change.Offer.DetailsUrl, Is.EqualTo("url b"));
        });
    }
    
    [Test]
    public void given_three_offers_and_then_there_should_be_one_offer_added_one_offer_removed_and_one_price_changed_detected()
    {
        var previousState = Create.Offers(
            new Offer("hotel a", "destination a", DateOnly.FromDayNumber(1), 8, "city a", 2, "url a"),
            new Offer("hotel b", "destination b", DateOnly.FromDayNumber(1), 8, "city a", 1, "url c"),
            new Offer("hotel c", "destination c", DateOnly.FromDayNumber(1), 8, "city a", 3, "url b"));
        
        var currentState = Create.Offers(
            new Offer("hotel a", "destination a", DateOnly.FromDayNumber(1), 8, "city a", 2000, "url a"),
            new Offer("hotel a", "destination a", DateOnly.FromDayNumber(2), 8, "city a", 2, "url b"),
            new Offer("hotel c", "destination c", DateOnly.FromDayNumber(1), 8, "city a", 3, "url b"));

        var changes = new OfferChangesDetector().DetectChanges(previousState, currentState);
        
        Assert.That(changes, Has.Count.EqualTo(3));
        var removedChange = changes.Single(c => c.ChangeType == OfferChangeType.OfferRemoved);
        var addedChange = changes.Single(c => c.ChangeType == OfferChangeType.OfferAdded);
        var priceChangedChange = changes.Single(c => c.ChangeType == OfferChangeType.PriceChanged);
        
        Assert.Multiple(() =>
        {
            Assert.That(removedChange.ChangeType, Is.EqualTo(OfferChangeType.OfferRemoved));
            Assert.That(removedChange.Offer.Hotel, Is.EqualTo("hotel b"));
            Assert.That(removedChange.Offer.Destination, Is.EqualTo("destination b"));
            Assert.That(removedChange.Offer.DepartureDate, Is.EqualTo(DateOnly.FromDayNumber(1)));
            Assert.That(removedChange.Offer.Days, Is.EqualTo(8));
            Assert.That(removedChange.Offer.CityOfDeparture, Is.EqualTo("city a"));
            Assert.That(removedChange.Offer.Price, Is.EqualTo(1));
            Assert.That(removedChange.Offer.DetailsUrl, Is.EqualTo("url c"));
            
            Assert.That(addedChange.ChangeType, Is.EqualTo(OfferChangeType.OfferAdded));
            Assert.That(addedChange.Offer.Hotel, Is.EqualTo("hotel a"));
            Assert.That(addedChange.Offer.Destination, Is.EqualTo("destination a"));
            Assert.That(addedChange.Offer.DepartureDate, Is.EqualTo(DateOnly.FromDayNumber(2)));
            Assert.That(addedChange.Offer.Days, Is.EqualTo(8));
            Assert.That(addedChange.Offer.CityOfDeparture, Is.EqualTo("city a"));
            Assert.That(addedChange.Offer.Price, Is.EqualTo(2));
            Assert.That(addedChange.Offer.DetailsUrl, Is.EqualTo("url b"));
            
            Assert.That(priceChangedChange.ChangeType, Is.EqualTo(OfferChangeType.PriceChanged));
            Assert.That(priceChangedChange.Offer.Hotel, Is.EqualTo("hotel a"));
            Assert.That(priceChangedChange.Offer.Destination, Is.EqualTo("destination a"));
            Assert.That(priceChangedChange.Offer.DepartureDate, Is.EqualTo(DateOnly.FromDayNumber(1)));
            Assert.That(priceChangedChange.Offer.Days, Is.EqualTo(8));
            Assert.That(priceChangedChange.Offer.CityOfDeparture, Is.EqualTo("city a"));
            Assert.That(priceChangedChange.Offer.Price, Is.EqualTo(2000));
            Assert.That(priceChangedChange.Offer.DetailsUrl, Is.EqualTo("url a"));
            Assert.That(priceChangedChange.OfferBeforeChange.Hotel, Is.EqualTo("hotel a"));
            Assert.That(priceChangedChange.OfferBeforeChange.Destination, Is.EqualTo("destination a"));
            Assert.That(priceChangedChange.OfferBeforeChange.DepartureDate, Is.EqualTo(DateOnly.FromDayNumber(1)));
            Assert.That(priceChangedChange.OfferBeforeChange.Days, Is.EqualTo(8));
            Assert.That(priceChangedChange.OfferBeforeChange.CityOfDeparture, Is.EqualTo("city a"));
            Assert.That(priceChangedChange.OfferBeforeChange.Price, Is.EqualTo(2));
            Assert.That(priceChangedChange.OfferBeforeChange.DetailsUrl, Is.EqualTo("url a"));
        });
    }
}
