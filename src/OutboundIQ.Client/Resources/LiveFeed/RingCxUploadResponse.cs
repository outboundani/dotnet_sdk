using System.Text.Json;
using System.Text.Json.Serialization;

namespace OutboundIQ;

/// <summary>
/// Response from <c>POST /live-feed/ringcx</c>.
/// </summary>
public sealed record RingCxUploadResponse
{
    /// <summary>Whether outboundIQ accepted the lead.</summary>
    [JsonPropertyName("success")]
    public bool Success { get; init; }

    /// <summary>The dialer's own response, passed through.</summary>
    [JsonPropertyName("dialerResponse")]
    public RingCxDialerResponse? DialerResponse { get; init; }

    /// <summary>Any properties the API returned that this SDK does not model yet.</summary>
    [JsonExtensionData]
    public IDictionary<string, JsonElement>? AdditionalProperties { get; set; }
}

/// <summary>
/// The RingCX dialer's response to a lead upload, passed through unchanged.
/// </summary>
public sealed record RingCxDialerResponse
{
    /// <summary>A human-readable description of the outcome.</summary>
    [JsonPropertyName("message")]
    public string? Message { get; init; }

    /// <summary>How many leads were submitted.</summary>
    [JsonPropertyName("leadsSupplied")]
    public double LeadsSupplied { get; init; }

    /// <summary>How many leads the dialer accepted.</summary>
    [JsonPropertyName("leadsAccepted")]
    public double LeadsAccepted { get; init; }

    /// <summary>How many leads the dialer inserted.</summary>
    [JsonPropertyName("leadsInserted")]
    public double LeadsInserted { get; init; }

    /// <summary>The dialer's processing result.</summary>
    [JsonPropertyName("processingResult")]
    public string? ProcessingResult { get; init; }

    /// <summary>The dialer's processing status.</summary>
    [JsonPropertyName("processingStatus")]
    public string? ProcessingStatus { get; init; }

    /// <summary>Any properties the API returned that this SDK does not model yet.</summary>
    [JsonExtensionData]
    public IDictionary<string, JsonElement>? AdditionalProperties { get; set; }
}
