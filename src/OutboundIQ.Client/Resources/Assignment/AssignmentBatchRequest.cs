using System.Text.Json.Serialization;

namespace OutboundIQ;

/// <summary>
/// Request body for <c>POST /assignment/batch</c>.
/// </summary>
public sealed record AssignmentBatchRequest
{
    /// <summary>The leads to assign ANIs to.</summary>
    [JsonPropertyName("leads")]
    public required IReadOnlyList<AssignmentBatchLead> Leads { get; init; }

    /// <summary>Set to <see langword="true"/> for real-time and interactive flows.</summary>
    [JsonPropertyName("real_time")]
    public bool? RealTime { get; init; }

    /// <summary>Return all ANIs in E.164 format. Defaults to <see langword="false"/>.</summary>
    [JsonPropertyName("e164")]
    public bool? E164 { get; init; }
}
