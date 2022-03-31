using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace DatePeriod;

/// <summary>
/// Converts a <c>DatePeriod</c> or value to or from JSON.
/// </summary>
public sealed class DatePeriodJsonConverter : JsonConverter<DatePeriod>
{
    private static ValueTuple<string, string> GetPropertyNames(JsonSerializerOptions options)
    {
        var startOnName = "StartOn";
        var endBeforeName = "EndBefore";
        if (options.PropertyNamingPolicy is not null)
        {
            startOnName = options.PropertyNamingPolicy.ConvertName(startOnName);
            endBeforeName = options.PropertyNamingPolicy.ConvertName(endBeforeName);
        }

        return (startOnName, endBeforeName);
    }

    private static DateOnly ReadDateOnlyProperty(ref Utf8JsonReader reader, string propertyName)
    {
        reader.Read();

        var propName = reader.GetString()!;
        if (propName != propertyName)
        {
            throw new JsonException($"DatePeriod: expected property name '{propertyName}', got '{propName}'");
        }

        reader.Read();
        if (reader.TokenType != JsonTokenType.String)
        {
            throw new JsonException(
                $"DatePeriod: expected string as {propertyName} property value, got {reader.TokenType}");
        }

        var dateOnlyStr = reader.GetString() ?? throw new JsonException($"DatePeriod: {propertyName} value was null");
        return DateOnly.ParseExact(dateOnlyStr, "yyyy-MM-dd", CultureInfo.InvariantCulture);
    }

    /// <inheritdoc />
    public override DatePeriod Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType != JsonTokenType.StartObject)
        {
            throw new JsonException("A DatePeriod should start with {");
        }

        var (startOnName, endBeforeName) = GetPropertyNames(options);

        var startOn = ReadDateOnlyProperty(ref reader, startOnName);
        var endBefore = ReadDateOnlyProperty(ref reader, endBeforeName);

        reader.Read();
        return new DatePeriod(startOn, endBefore);
    }

    /// <inheritdoc />
    public override void Write(Utf8JsonWriter writer, DatePeriod value, JsonSerializerOptions options)
    {
        var (startOn, endBefore) = value;
        var (startOnName, endBeforeName) = GetPropertyNames(options);
        writer.WriteStartObject();
        writer.WriteString(startOnName, startOn.ToISOString());
        writer.WriteString(endBeforeName, endBefore.ToISOString());
        writer.WriteEndObject();
    }
}