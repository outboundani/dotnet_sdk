using System.Text.Json.Serialization;

namespace OutboundIQ;

/// <summary>
/// Response from <c>POST /assignment/batch</c>.
/// </summary>
public sealed record AssignmentBatchResponse
{
    /// <summary>Whether the batch as a whole was accepted.</summary>
    [JsonPropertyName("success")]
    public bool Success { get; init; }

    /// <summary>One result per submitted lead, in no guaranteed order.</summary>
    [JsonPropertyName("results")]
    public IReadOnlyList<AssignmentBatchResult> Results { get; init; } = [];
}
