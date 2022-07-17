using Holidays.Eventing.Core;

namespace Holidays.Eventing.RabbitMq.Tests;

internal class TestEvent : IEvent
{
    public TestEvent(
        DateTime timestamp,
        int intValue,
        string stringValue)
    {
        Timestamp = timestamp;
        IntValue = intValue;
        StringValue = stringValue;
    }

    public int IntValue { get; }

    public string StringValue { get; }

    public DateTime Timestamp { get; }
}
