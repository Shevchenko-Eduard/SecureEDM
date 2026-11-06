using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace SecureEDM.Client.Application.Models;

/// <summary>Принимает дату как ISO-строку или как Unix-время в миллисекундах (формат HTML-прототипа).</summary>
public sealed class FlexibleDateTimeOffsetConverter : JsonConverter<DateTimeOffset>
{
    public override DateTimeOffset Read(
        ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.Number)
            return DateTimeOffset.FromUnixTimeMilliseconds(reader.GetInt64());

        if (reader.TokenType == JsonTokenType.String)
        {
            var value = reader.GetString();
            if (DateTimeOffset.TryParse(
                    value,
                    CultureInfo.InvariantCulture,
                    DateTimeStyles.RoundtripKind,
                    out var result))
                return result;
        }

        throw new JsonException("Ожидалась дата в формате ISO 8601 или Unix-время в миллисекундах.");
    }

    public override void Write(
        Utf8JsonWriter writer, DateTimeOffset value, JsonSerializerOptions options) =>
        writer.WriteStringValue(value);
}
