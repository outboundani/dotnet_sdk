using System.Text.Json;
using System.Text.Json.Serialization;

namespace OutboundIQ;

/// <summary>
/// Response from <c>GET /custom/anis</c>.
/// </summary>
public sealed record GetCustomAniResponse
{
    /// <summary>Whether the ANI was found.</summary>
    [JsonPropertyName("success")]
    public bool Success { get; init; }

    /// <summary>The ANI record.</summary>
    [JsonPropertyName("ani")]
    public CustomAniDetails? Ani { get; init; }

    /// <summary>Any properties the API returned that this SDK does not model yet.</summary>
    [JsonExtensionData]
    public IDictionary<string, JsonElement>? AdditionalProperties { get; set; }
}
