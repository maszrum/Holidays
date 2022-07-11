namespace Holidays.Postgres.DbRecords;

internal class OfferEventLogRecord
{
    // ReSharper disable once UnusedMember.Local
    private OfferEventLogRecord()
    {
    }
    
    public OfferEventLogRecord(
        Guid id, 
        Guid offerId, 
        string eventType, 
        string @params)
    {
        Id = id;
        OfferId = offerId;
        EventType = eventType;
        Params = @params;
    }

    public Guid Id { get; init; }
    
    public Guid OfferId { get; init; }

    public string EventType { get; init; } = null!;

    public string Params { get; init; } = null!;
}
