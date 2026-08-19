using System.Text.Json.Serialization;

namespace OutboundIQ;

/// <summary>
/// The outcome for a single lead in a batch assignment.
/// </summary>
/// <remarks>
/// A batch can partially succeed, so check <see cref="Error"/> on every row rather than relying on
/// the overall <see cref="AssignmentBatchResponse.Success"/> flag.
/// </remarks>
public sealed record AssignmentBatchResult
{
    /// <summary>The <see cref="AssignmentBatchLead.RowId"/> this result corresponds to.</summary>
    [JsonPropertyName("row_id")]
    public string RowId { get; init; } = string.Empty;

    /// <summary>The assigned ANI, or an empty string when assignment failed for this lead.</summary>
    [JsonPropertyName("outboundani")]
    public string OutboundAni { get; init; } = string.Empty;

    /// <summary>An empty string on success, otherwise the reason this lead failed.</summary>
    [JsonPropertyName("error")]
    public string Error { get; init; } = string.Empty;
}
