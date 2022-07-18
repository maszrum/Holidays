using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Holidays.Eventing.RabbitMq;

internal class DateOnlyJsonConverter : JsonConverter<DateOnly>
{
    public override DateOnly Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var dateString = reader.GetString();

        return dateString is null
            ? DateOnly.FromDayNumber(0)
            : DateOnly.Parse(dateString, CultureInfo.InvariantCulture);
    }

    public override void Write(Utf8JsonWriter writer, DateOnly value, JsonSerializerOptions options)
    {
        if (value.DayNumber == 0)
        {
            writer.WriteNullValue();
        }
        else
        {
            var dateString = value.ToString(CultureInfo.InvariantCulture);
            writer.WriteStringValue(dateString);
        }
    }
}
