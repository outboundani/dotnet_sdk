using System.Text.Json.Serialization;

namespace OutboundIQ;

/// <summary>
/// A partial disposition update for <c>PUT /custom/dispos</c>. Omitted properties are left
/// unchanged.
/// </summary>
public sealed record CustomDispoUpdate
{
    /// <summary>Your dialer's identifier for the disposition to update.</summary>
    [JsonPropertyName("id")]
    public required string Id { get; init; }

    /// <summary>The new disposition name.</summary>
    [JsonPropertyName("name")]
    public string? Name { get; init; }

    /// <summary>The new disposition type. See <see cref="CustomDispoTypes"/>.</summary>
    [JsonPropertyName("type")]
    public string? Type { get; init; }

    /// <summary>Whether this disposition counts as a contact.</summary>
    [JsonPropertyName("contact")]
    public bool? Contact { get; init; }

    /// <summary>Whether this disposition counts as a success.</summary>
    [JsonPropertyName("success")]
    public bool? Success { get; init; }
}
