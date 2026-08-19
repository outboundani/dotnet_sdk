using System.Text.Json;
using System.Text.Json.Serialization;

namespace OutboundIQ;

/// <summary>
/// Response from the NRM remediate, pause, and activate endpoints.
/// </summary>
/// <remarks>
/// The shape varies by outcome — <c>{ "status": "paused", "ani": "..." }</c> on success, or
/// <c>{ "success": false, "message": "..." }</c> on failure. Note that
/// <see cref="INrmResource.RemediateAsync"/> can return HTTP 200 with
/// <c>{ "status": "within cooldown" }</c>, which is a refusal despite the success status code.
/// </remarks>
public sealed record NrmActionResponse
{
    /// <summary>The resulting state, such as <c>"paused"</c> or <c>"within cooldown"</c>.</summary>
    [JsonPropertyName("status")]
    public string? Status { get; init; }

    /// <summary>The ANI the action applied to.</summary>
    [JsonPropertyName("ani")]
    public string? Ani { get; init; }

    /// <summary>Whether the action succeeded, when the API reports it explicitly.</summary>
    [JsonPropertyName("success")]
    public bool? Success { get; init; }

    /// <summary>A human-readable description of the outcome.</summary>
    [JsonPropertyName("message")]
    public string? Message { get; init; }

    /// <summary>Any properties the API returned that this SDK does not model yet.</summary>
    [JsonExtensionData]
    public IDictionary<string, JsonElement>? AdditionalProperties { get; set; }
}
