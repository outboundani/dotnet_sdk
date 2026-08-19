using System.Buffers;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace OutboundIQ;

/// <summary>
/// Reads and writes <see cref="NumberOrString"/>, preserving whether the value was carried as a
/// JSON number or a JSON string.
/// </summary>
/// <remarks>
/// Public because <see cref="NumberOrString"/> is public and names this converter in a
/// <see cref="JsonConverterAttribute"/>. The System.Text.Json source generator must be able to
/// reach it from any assembly that builds a serializer context over these models.
/// </remarks>
public sealed class NumberOrStringConverter : JsonConverter<NumberOrString>
{
    /// <summary>Reads a value that may be a JSON number or a JSON string.</summary>
    /// <param name="reader">The reader.</param>
    /// <param name="typeToConvert">The target type.</param>
    /// <param name="options">The serializer options.</param>
    /// <returns>The parsed value.</returns>
    public override NumberOrString Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) =>
        reader.TokenType switch
        {
            JsonTokenType.Null => default,
            JsonTokenType.String => NumberOrString.FromString(reader.GetString()),
            JsonTokenType.Number => NumberOrString.FromRawNumber(ReadRawToken(ref reader)),
            JsonTokenType.True => NumberOrString.FromString("true"),
            JsonTokenType.False => NumberOrString.FromString("false"),
            _ => throw new JsonException(
                $"Expected a number or a string for {nameof(NumberOrString)}, but found {reader.TokenType}."),
        };

    /// <summary>Writes the value back in the form it arrived in.</summary>
    /// <param name="writer">The writer.</param>
    /// <param name="value">The value to write.</param>
    /// <param name="options">The serializer options.</param>
    public override void Write(Utf8JsonWriter writer, NumberOrString value, JsonSerializerOptions options)
    {
        if (!value.HasValue)
        {
            writer.WriteNullValue();
            return;
        }

        if (value.IsNumber)
        {
            // The text came from a JSON number token or from our own invariant formatting, so it
            // is known to be valid JSON already.
            writer.WriteRawValue(value.RawText!, skipInputValidation: true);
            return;
        }

        writer.WriteStringValue(value.Text);
    }

    /// <summary>
    /// Copies the raw token text, so a large integer or a high-precision decimal survives the
    /// round trip that parsing to <see cref="double"/> would quantize.
    /// </summary>
    private static string ReadRawToken(ref Utf8JsonReader reader)
    {
        var bytes = reader.HasValueSequence ? reader.ValueSequence.ToArray() : reader.ValueSpan.ToArray();
        return Encoding.UTF8.GetString(bytes);
    }
}
