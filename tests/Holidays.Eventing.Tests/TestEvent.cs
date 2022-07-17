using Holidays.Eventing.Core;

namespace Holidays.Eventing.Tests;

internal class TestEvent : IEvent
{
    public DateTime Timestamp => new(2022, 7, 11, 17, 57, 12);
}
