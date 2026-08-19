using System.Text.Json.Serialization;

namespace OutboundIQ;

/// <summary>
/// Request body for <c>POST /live-feed/ringcx</c>.
/// </summary>
public sealed record RingCxUploadRequest
{
    /// <summary>The RingCX campaign to push the lead into.</summary>
    [JsonPropertyName("campaignId")]
    public required string CampaignId { get; init; }

    /// <summary>The lead.</summary>
    [JsonPropertyName("lead")]
    public required RingCxLead Lead { get; init; }

    /// <summary>Optional dialer behavior.</summary>
    [JsonPropertyName("options")]
    public RingCxUploadOptions? Options { get; init; }
}
