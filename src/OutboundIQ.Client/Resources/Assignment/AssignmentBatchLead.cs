using System.Text.Json.Serialization;

namespace OutboundIQ;

/// <summary>
/// One lead in an <see cref="AssignmentBatchRequest"/>.
/// </summary>
public sealed record AssignmentBatchLead
{
    /// <summary>
    /// Caller-supplied row identifier, echoed back on the matching
    /// <see cref="AssignmentBatchResult"/>.
    /// </summary>
    [JsonPropertyName("row_id")]
    public required string RowId { get; init; }

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
}
