namespace Holidays.DataTransferObjects;

public class PriceHistoryDto
{
    public Guid OfferId { get; init; }

    public IReadOnlyList<PriceHistoryEntryDto> History { get; init; } = null!;
}

public class PriceHistoryEntryDto
{
    public DateTime Timestamp { get; init; }

    public int Price { get; init; }
}
