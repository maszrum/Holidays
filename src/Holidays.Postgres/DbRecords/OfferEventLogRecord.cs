namespace Holidays.Postgres.DbRecords;

internal class OfferEventLogRecord
{
    // ReSharper disable once UnusedMember.Local
    private OfferEventLogRecord()
    {
    }
    
    public OfferEventLogRecord(
        Guid id, 
        DateTime eventTimestamp,
        Guid offerId, 
        string eventType, 
        string @params)
    {
        Id = id;
        EventTimestamp = eventTimestamp;
        OfferId = offerId;
        EventType = eventType;
        Params = @params;
    }

    public Guid Id { get; init; }
    
    public DateTime EventTimestamp { get; init; }
    
    public Guid OfferId { get; init; }

    public string EventType { get; init; } = null!;

    public string Params { get; init; } = null!;
}
