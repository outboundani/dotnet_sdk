using System.Text.Json;
using System.Text.Json.Serialization;

namespace OutboundIQ;

/// <summary>
/// The acknowledgement envelope returned by the Custom Dialer endpoints.
/// </summary>
/// <remarks>
/// The documentation does not pin down every response shape for this API, so any properties not
/// listed here are preserved in <see cref="AdditionalProperties"/> rather than discarded.
/// </remarks>
public sealed record CustomApiResponse
{
    /// <summary>Whether the operation succeeded.</summary>
    [JsonPropertyName("success")]
    public bool? Success { get; init; }

    /// <summary>A human-readable description of the outcome.</summary>
    [JsonPropertyName("message")]
    public string? Message { get; init; }

    /// <summary>Any properties the API returned that this SDK does not model yet.</summary>
    /// <remarks>
    /// This uses <c>set</c> rather than <c>init</c> deliberately: the System.Text.Json source
    /// generator cannot deserialize into an init-only extension data property.
    /// </remarks>
    [JsonExtensionData]
    public IDictionary<string, JsonElement>? AdditionalProperties { get; set; }
}
