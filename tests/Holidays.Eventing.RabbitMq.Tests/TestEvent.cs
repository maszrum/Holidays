using Holidays.Core.Eventing;

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

    public DateTime Timestamp { get; }
    
    public int IntValue { get; }
    
    public string StringValue { get; }
}
