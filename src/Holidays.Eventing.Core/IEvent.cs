namespace Holidays.Eventing.Core;

public interface IEvent
{
    DateTime Timestamp { get; }
}
