namespace Holidays.Core.Eventing;

public interface IEvent
{
    DateTime Timestamp { get; }
}
