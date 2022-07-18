using Holidays.Eventing.Core;

namespace Holidays.Eventing.RabbitMq.Tests;

internal class TestEvent : IEvent
{
    public TestEvent(
        DateTime timestamp,
        int intValue,
        string stringValue,
        DateOnly dateOnlyValue)
    {
        Timestamp = timestamp;
        IntValue = intValue;
        StringValue = stringValue;
        DateOnlyValue = dateOnlyValue;
    }

    public int IntValue { get; }

    public string StringValue { get; }

    public DateOnly DateOnlyValue { get; }

    public DateTime Timestamp { get; }
}
