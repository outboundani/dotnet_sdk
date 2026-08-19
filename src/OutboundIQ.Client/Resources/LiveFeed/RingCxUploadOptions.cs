using System.Text.Json.Serialization;

namespace OutboundIQ;

/// <summary>
/// Optional dialer behavior for a RingCX lead upload.
/// </summary>
public sealed record RingCxUploadOptions
{
    /// <summary>When the lead should be dialed. See <see cref="RingCxDialPriorities"/>.</summary>
    [JsonPropertyName("dialPriority")]
    public string? DialPriority { get; init; }

    /// <summary>
    /// How to treat existing copies of this lead. See <see cref="RingCxDuplicateHandlings"/>.
    /// </summary>
    [JsonPropertyName("duplicateHandling")]
    public string? DuplicateHandling { get; init; }

    /// <summary>Whether the target list is dialable. See <see cref="RingCxListStates"/>.</summary>
    [JsonPropertyName("listState")]
    public string? ListState { get; init; }

    /// <summary>
    /// How to derive the prospect's time zone. See <see cref="RingCxTimeZoneOptions"/>.
    /// </summary>
    [JsonPropertyName("timeZoneOption")]
    public string? TimeZoneOption { get; init; }

    /// <summary>A description recorded against the upload.</summary>
    [JsonPropertyName("description")]
    public string? Description { get; init; }
}
