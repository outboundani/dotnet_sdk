using System.Text.Json.Serialization;

namespace OutboundIQ;

/// <summary>
/// Response from <c>POST /dials</c>.
/// </summary>
public sealed record DialQueuedResponse
{
    /// <summary>An acknowledgement, typically <c>"dial queued"</c>.</summary>
    [JsonPropertyName("message")]
    public string Message { get; init; } = string.Empty;
}
