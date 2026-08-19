using System.Text.Json.Serialization;

namespace OutboundIQ;

/// <summary>
/// An outbound campaign of the company, with its mapped inbound campaigns.
/// </summary>
public sealed record AniPlannerCampaign
{
    /// <summary>The numeric campaign identifier.</summary>
    [JsonPropertyName("id")]
    public long Id { get; init; }

    /// <summary>The campaign name.</summary>
    [JsonPropertyName("name")]
    public string? Name { get; init; }

    /// <summary>
    /// The value to pass in <see cref="AniPlannerGenerateRequest.Campaigns"/> or
    /// <see cref="AniPlannerGenerateRequest.CampaignsExclude"/>.
    /// </summary>
    [JsonPropertyName("internalIdentifier")]
    public string? InternalIdentifier { get; init; }

    /// <summary>The campaign type.</summary>
    [JsonPropertyName("campaignType")]
    public string? CampaignType { get; init; }

    /// <summary>Identifiers of the inbound campaigns mapped to this one.</summary>
    [JsonPropertyName("inboundCampaignIds")]
    public IReadOnlyList<long> InboundCampaignIds { get; init; } = [];

    /// <summary>Names of the inbound campaigns mapped to this one.</summary>
    [JsonPropertyName("inboundCampaignNames")]
    public IReadOnlyList<string> InboundCampaignNames { get; init; } = [];
}
