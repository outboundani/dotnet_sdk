using System.Text.Json;
using System.Text.Json.Serialization;

namespace OutboundIQ;

/// <summary>
/// Response from <c>GET /nrm/anis</c>.
/// </summary>
/// <remarks>
/// Note the mixed casing: the envelope is snake_case while the ANI rows inside
/// <see cref="Data"/> are camelCase. That is how the API returns it.
/// </remarks>
public sealed record NrmListAnisResponse
{
    /// <summary>The outcome, typically <c>"success"</c>.</summary>
    [JsonPropertyName("result")]
    public string? Result { get; init; }

    /// <summary>How many ANIs are on this page.</summary>
    [JsonPropertyName("count")]
    public int Count { get; init; }

    /// <summary>How many ANIs match the filter in total.</summary>
    [JsonPropertyName("total_anis")]
    public int TotalAnis { get; init; }

    /// <summary>Whether another page follows this one.</summary>
    [JsonPropertyName("can_next_page")]
    public bool CanNextPage { get; init; }

    /// <summary>Whether a page precedes this one.</summary>
    [JsonPropertyName("can_prev_page")]
    public bool CanPrevPage { get; init; }

    /// <summary>How many pages there are in total.</summary>
    [JsonPropertyName("total_pages")]
    public int TotalPages { get; init; }

    /// <summary>The ANIs on this page.</summary>
    [JsonPropertyName("data")]
    public IReadOnlyList<NrmAni> Data { get; init; } = [];

    /// <summary>Any properties the API returned that this SDK does not model yet.</summary>
    [JsonExtensionData]
    public IDictionary<string, JsonElement>? AdditionalProperties { get; set; }
}
