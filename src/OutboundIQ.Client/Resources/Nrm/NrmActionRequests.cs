using System.Text.Json.Serialization;

namespace OutboundIQ;

/// <summary>
/// Request body for <c>POST /nrm/remediate</c>.
/// </summary>
public sealed record NrmRemediateRequest
{
    /// <summary>The ANI to remediate.</summary>
    [JsonPropertyName("ani")]
    public required string Ani { get; init; }

    /// <summary>
    /// Six digit carrier identifier. The full list is published at
    /// <see href="https://docs.outboundiq.cloud/reference/nrm/carriers/"/>.
    /// </summary>
    [JsonPropertyName("carrier")]
    public string? Carrier { get; init; }

    /// <summary>A free-text note recorded with the request.</summary>
    [JsonPropertyName("note")]
    public string? Note { get; init; }
}

/// <summary>
/// Request body for <c>POST /nrm/pause</c>.
/// </summary>
public sealed record NrmPauseRequest
{
    /// <summary>The ANI to pause.</summary>
    [JsonPropertyName("ani")]
    public required string Ani { get; init; }

    /// <summary>Six digit carrier identifier.</summary>
    [JsonPropertyName("carrier")]
    public string? Carrier { get; init; }

    /// <summary>A free-text note recorded with the request.</summary>
    [JsonPropertyName("note")]
    public string? Note { get; init; }

    /// <summary>When the cooling-off period should start, as <c>"YYYY-MM-DD"</c>.</summary>
    [JsonPropertyName("date")]
    public string? Date { get; init; }
}

/// <summary>
/// Request body for <c>POST /nrm/activate</c>.
/// </summary>
public sealed record NrmActivateRequest
{
    /// <summary>The ANI to return to active dialing.</summary>
    [JsonPropertyName("ani")]
    public required string Ani { get; init; }

    /// <summary>When to reactivate, as <c>"YYYY-MM-DD"</c>. Required by this endpoint.</summary>
    [JsonPropertyName("date")]
    public required string Date { get; init; }

    /// <summary>Six digit carrier identifier.</summary>
    [JsonPropertyName("carrier")]
    public string? Carrier { get; init; }

    /// <summary>A free-text note recorded with the request.</summary>
    [JsonPropertyName("note")]
    public string? Note { get; init; }
}
