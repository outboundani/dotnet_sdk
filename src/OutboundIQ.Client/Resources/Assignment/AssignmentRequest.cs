using System.Text.Json.Serialization;

namespace OutboundIQ;

/// <summary>
/// Request body for <c>POST /assignment</c>.
/// </summary>
public sealed record AssignmentRequest
{
    /// <summary>Prospect phone number, 10 to 15 digits.</summary>
    [JsonPropertyName("prospect_phone")]
    public required string ProspectPhone { get; init; }

    /// <summary>US zip code used for geographic matching.</summary>
    [JsonPropertyName("prospect_zip")]
    public string? ProspectZip { get; init; }

    /// <summary>
    /// Campaign identifier. Optional when a default campaign is configured for the key.
    /// </summary>
    [JsonPropertyName("dialer_campaign")]
    public string? DialerCampaign { get; init; }

    /// <summary>Set to <see langword="true"/> for real-time and interactive flows.</summary>
    [JsonPropertyName("real_time")]
    public bool? RealTime { get; init; }

    /// <summary>Return the ANI in E.164 format. Defaults to <see langword="false"/>.</summary>
    [JsonPropertyName("e164")]
    public bool? E164 { get; init; }
}
