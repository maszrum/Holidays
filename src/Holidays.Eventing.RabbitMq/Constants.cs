namespace Holidays.Eventing.RabbitMq;

internal static class Constants
{
    public const string Exchange = "events";

    public const string QueuePrefix = "events-";

    public const string EventTypeHeader = "event-type";

    public const string EventPublisherHeader = "event-publisher";
}
