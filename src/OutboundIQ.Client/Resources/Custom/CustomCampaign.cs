using System.Text.Json.Serialization;

namespace OutboundIQ;

/// <summary>
/// A campaign to create through <c>POST /custom/campaigns</c>.
/// </summary>
public sealed record CustomCampaign
{
    /// <summary>Your dialer's identifier for the campaign.</summary>
    [JsonPropertyName("id")]
    public required string Id { get; init; }

    /// <summary>The campaign name.</summary>
    [JsonPropertyName("name")]
    public required string Name { get; init; }

    /// <summary>The campaign type. See <see cref="CustomCampaignTypes"/>.</summary>
    [JsonPropertyName("type")]
    public required string Type { get; init; }

    /// <summary>The inbound pool this campaign draws from, when applicable.</summary>
    [JsonPropertyName("inbound_pool_id")]
    public string? InboundPoolId { get; init; }
}
