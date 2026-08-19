using System.Text.Json.Serialization;

namespace OutboundIQ;

/// <summary>
/// The body of a DELETE that identifies its target by <c>id</c>.
/// </summary>
/// <remarks>
/// The Custom Dialer delete endpoints take their identifier in the request body rather than the
/// query string, which is why these calls send content on a DELETE.
/// </remarks>
internal sealed record CustomIdBody
{
    [JsonPropertyName("id")]
    public required string Id { get; init; }
}

/// <summary>
/// The body of a DELETE that identifies its target by <c>number</c>.
/// </summary>
internal sealed record CustomNumberBody
{
    [JsonPropertyName("number")]
    public required string Number { get; init; }
}
