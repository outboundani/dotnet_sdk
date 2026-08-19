using System.Text.Json.Serialization;

namespace OutboundIQ;

/// <summary>
/// A single dial record for <c>POST /dials</c>.
/// </summary>
/// <remarks>
/// On outbound calls <see cref="FromNumber"/> is the caller ID and <see cref="ToNumber"/> is the
/// prospect. On inbound calls the two are reversed.
/// </remarks>
public sealed record DialRecord
{
    /// <summary>Your dialer's identifier for the campaign.</summary>
    [JsonPropertyName("campaign_id")]
    public required string CampaignId { get; init; }

    /// <summary>The campaign name.</summary>
    [JsonPropertyName("campaign_name")]
    public required string CampaignName { get; init; }

    /// <summary>The agent who handled the call.</summary>
    [JsonPropertyName("agent_name")]
    public required string AgentName { get; init; }

    /// <summary>Caller ID on outbound calls, prospect number on inbound calls.</summary>
    [JsonPropertyName("from_number")]
    public required string FromNumber { get; init; }

    /// <summary>Prospect number on outbound calls, caller ID on inbound calls.</summary>
    [JsonPropertyName("to_number")]
    public required string ToNumber { get; init; }

    /// <summary>The disposition applied to the call.</summary>
    [JsonPropertyName("disposition_name")]
    public required string DispositionName { get; init; }

    /// <summary>When the call happened, as <c>"YYYY-MM-DD HH:MM:SS"</c> or an ISO 8601 timestamp.</summary>
    [JsonPropertyName("datetime")]
    public required string DateTime { get; init; }

    /// <summary>The call direction. See <see cref="CallDirections"/> for the documented values.</summary>
    [JsonPropertyName("call_direction")]
    public required string CallDirection { get; init; }

    /// <summary>Prospect zip code. Five digits preferred.</summary>
    [JsonPropertyName("zip")]
    public required string Zip { get; init; }

    /// <summary>Date the lead was originally created, as <c>"YYYY-MM-DD"</c>.</summary>
    [JsonPropertyName("sys_created_date_original")]
    public required string SysCreatedDateOriginal { get; init; }

    /// <summary>
    /// How many times this lead has been dialed. The API accepts either a number or a string.
    /// </summary>
    [JsonPropertyName("total_dial_attempts")]
    public required NumberOrString TotalDialAttempts { get; init; }

    /// <summary>The skill or queue the call was routed through.</summary>
    [JsonPropertyName("skill_name")]
    public required string SkillName { get; init; }

    /// <summary>Where the lead came from.</summary>
    [JsonPropertyName("lead_source")]
    public required string LeadSource { get; init; }

    /// <summary>
    /// Optional unique identifier for this dial. Supply one to make retries idempotent on
    /// outboundIQ's side.
    /// </summary>
    [JsonPropertyName("dial_id")]
    public string? DialId { get; init; }
}
