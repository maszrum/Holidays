using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Holidays.WebAPI.Json;

internal class DateOnlyJsonConverter : JsonConverter<DateOnly>
{
    public override DateOnly Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var stringValue = reader.GetString();

        if (stringValue is null)
        {
            return new DateOnly();
        }

        var deserialized = DateOnly.Parse(stringValue, CultureInfo.InvariantCulture);
        return deserialized;
    }

    public override void Write(Utf8JsonWriter writer, DateOnly value, JsonSerializerOptions options)
    {
        if (value.DayNumber == 0)
        {
            writer.WriteNullValue();
        }
        else
        {
            var serialized = value.ToString(CultureInfo.InvariantCulture);
            writer.WriteStringValue(serialized);
        }
    }
}
