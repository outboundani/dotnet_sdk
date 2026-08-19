using System.Text.Json.Serialization;

namespace OutboundIQ;

/// <summary>
/// A single dial, as delivered inside a <see cref="DialBatchWebhookPayload"/>.
/// </summary>
public sealed record DialEvent
{
    /// <summary>The company the dial belongs to.</summary>
    [JsonPropertyName("companySlug")]
    public string? CompanySlug { get; init; }

    /// <summary>The dialer's call identifier.</summary>
    [JsonPropertyName("callId")]
    public string? CallId { get; init; }

    /// <summary>The call direction. See <see cref="CallDirections"/>.</summary>
    [JsonPropertyName("callDirection")]
    public string? CallDirection { get; init; }

    /// <summary>When the call happened, as an ISO 8601 UTC timestamp.</summary>
    [JsonPropertyName("timestamp")]
    public string? Timestamp { get; init; }

    /// <summary>The caller ID used.</summary>
    [JsonPropertyName("ani")]
    public string? Ani { get; init; }

    /// <summary>The prospect's number.</summary>
    [JsonPropertyName("phone")]
    public string? Phone { get; init; }

    /// <summary>The campaign name.</summary>
    [JsonPropertyName("campaign")]
    public string? Campaign { get; init; }

    /// <summary>The campaign's internal identifier.</summary>
    [JsonPropertyName("campaignInternalId")]
    public string? CampaignInternalId { get; init; }

    /// <summary>The agent who handled the call.</summary>
    [JsonPropertyName("agent")]
    public string? Agent { get; init; }

    /// <summary>The disposition applied.</summary>
    [JsonPropertyName("disposition")]
    public string? Disposition { get; init; }

    /// <summary>The disposition's identifier.</summary>
    [JsonPropertyName("dispositionId")]
    public string? DispositionId { get; init; }

    /// <summary>Whether the disposition counts as a contact.</summary>
    [JsonPropertyName("contact")]
    public bool Contact { get; init; }

    /// <summary>Whether the disposition counts as a success.</summary>
    [JsonPropertyName("success")]
    public bool Success { get; init; }

    /// <summary>Whether the disposition was applied by the dialer rather than an agent.</summary>
    [JsonPropertyName("isSystemDispo")]
    public bool IsSystemDispo { get; init; }

    /// <summary>How many times this lead had been dialed.</summary>
    [JsonPropertyName("totalDialAttempts")]
    public double? TotalDialAttempts { get; init; }
}
