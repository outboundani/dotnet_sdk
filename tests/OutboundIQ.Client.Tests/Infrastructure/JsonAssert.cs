using System.Text.Json;

namespace OutboundIQ.Tests;

/// <summary>
/// Order-insensitive deep comparison of two JSON documents.
/// </summary>
/// <remarks>
/// Hand-rolled rather than using <c>JsonNode.DeepEquals</c>, which is .NET 9+ and so would not
/// compile for the net8.0 target.
/// </remarks>
internal static class JsonAssert
{
    public static void Equivalent(string expected, string? actual)
    {
        Assert.NotNull(actual);

        using var expectedDocument = JsonDocument.Parse(expected);
        using var actualDocument = JsonDocument.Parse(actual!);

        if (!DeepEquals(expectedDocument.RootElement, actualDocument.RootElement))
        {
            Assert.Fail($"JSON did not match.{Environment.NewLine}Expected: {Minify(expected)}{Environment.NewLine}Actual:   {Minify(actual!)}");
        }
    }

    private static bool DeepEquals(JsonElement left, JsonElement right)
    {
        if (left.ValueKind != right.ValueKind)
        {
            return false;
        }

        switch (left.ValueKind)
        {
            case JsonValueKind.Object:
                var leftProperties = left.EnumerateObject().ToDictionary(p => p.Name, p => p.Value, StringComparer.Ordinal);
                var rightProperties = right.EnumerateObject().ToDictionary(p => p.Name, p => p.Value, StringComparer.Ordinal);

                return leftProperties.Count == rightProperties.Count
                    && leftProperties.All(pair =>
                        rightProperties.TryGetValue(pair.Key, out var other) && DeepEquals(pair.Value, other));

            case JsonValueKind.Array:
                var leftItems = left.EnumerateArray().ToList();
                var rightItems = right.EnumerateArray().ToList();

                // Arrays stay order-sensitive: order is meaningful in every payload here.
                return leftItems.Count == rightItems.Count
                    && leftItems.Zip(rightItems).All(pair => DeepEquals(pair.First, pair.Second));

            case JsonValueKind.String:
                return string.Equals(left.GetString(), right.GetString(), StringComparison.Ordinal);

            case JsonValueKind.Number:
                return left.GetDecimal() == right.GetDecimal();

            default:
                // true, false, null, and undefined are fully determined by ValueKind.
                return true;
        }
    }

    private static string Minify(string json)
    {
        using var document = JsonDocument.Parse(json);
        return JsonSerializer.Serialize(document.RootElement);
    }
}
