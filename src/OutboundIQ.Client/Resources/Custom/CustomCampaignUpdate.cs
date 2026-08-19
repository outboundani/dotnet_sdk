using System.Text.Json.Serialization;

namespace OutboundIQ;

/// <summary>
/// A partial campaign update for <c>PUT /custom/campaigns</c>. Omitted properties are left
/// unchanged.
/// </summary>
public sealed record CustomCampaignUpdate
{
    /// <summary>Your dialer's identifier for the campaign to update.</summary>
    [JsonPropertyName("id")]
    public required string Id { get; init; }

    /// <summary>The new campaign name.</summary>
    [JsonPropertyName("name")]
    public string? Name { get; init; }

    /// <summary>The new campaign type. See <see cref="CustomCampaignTypes"/>.</summary>
    [JsonPropertyName("type")]
    public string? Type { get; init; }

    /// <summary>The new inbound pool.</summary>
    [JsonPropertyName("inbound_pool_id")]
    public string? InboundPoolId { get; init; }
}
