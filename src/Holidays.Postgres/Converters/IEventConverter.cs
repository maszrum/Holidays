using System.Diagnostics.CodeAnalysis;
using Holidays.Core.Eventing;
using Holidays.Postgres.DbRecords;

namespace Holidays.Postgres.Converters;

internal interface IEventConverter
{
    bool TryConvertToRecord(IEvent @event, [NotNullWhen(true)] out OfferEventLogRecord? record);

    IEvent ConvertToObject(OfferEventLogRecord record);
}
