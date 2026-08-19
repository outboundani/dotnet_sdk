using System.Text.Json.Serialization;

namespace OutboundIQ;

/// <summary>
/// A disposition to create through <c>POST /custom/dispos</c>.
/// </summary>
public sealed record CustomDispo
{
    /// <summary>Your dialer's identifier for the disposition.</summary>
    [JsonPropertyName("id")]
    public required string Id { get; init; }

    /// <summary>The disposition name.</summary>
    [JsonPropertyName("name")]
    public required string Name { get; init; }

    /// <summary>The disposition type. See <see cref="CustomDispoTypes"/>.</summary>
    [JsonPropertyName("type")]
    public required string Type { get; init; }

    /// <summary>Whether this disposition counts as a contact.</summary>
    [JsonPropertyName("contact")]
    public required bool Contact { get; init; }

    /// <summary>Whether this disposition counts as a success.</summary>
    [JsonPropertyName("success")]
    public required bool Success { get; init; }
}
