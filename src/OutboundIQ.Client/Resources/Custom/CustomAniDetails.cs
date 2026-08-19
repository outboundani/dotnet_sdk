using System.Text.Json;
using System.Text.Json.Serialization;

namespace OutboundIQ;

/// <summary>
/// An ANI as returned by <c>GET /custom/anis</c>.
/// </summary>
/// <remarks>
/// Carries the same fields as <see cref="CustomAni"/> plus whatever else the API includes, kept in
/// <see cref="AdditionalProperties"/>.
/// </remarks>
public sealed record CustomAniDetails
{
    /// <summary>The country calling code. See <see cref="CountryCodes"/>.</summary>
    [JsonPropertyName("country_code")]
    public string? CountryCode { get; init; }

    /// <summary>The phone number, stored as digits only.</summary>
    [JsonPropertyName("number")]
    public string? Number { get; init; }

    /// <summary>The inbound group this number routes to.</summary>
    [JsonPropertyName("inbound_group_id")]
    public string? InboundGroupId { get; init; }

    /// <summary>Whether the number carries branded caller ID.</summary>
    [JsonPropertyName("is_branded")]
    public bool? IsBranded { get; init; }

    /// <summary>The brand displayed on branded calls.</summary>
    [JsonPropertyName("brand_name")]
    public string? BrandName { get; init; }

    /// <summary>Six digit carrier identifier.</summary>
    [JsonPropertyName("carrier_id")]
    public string? CarrierId { get; init; }

    /// <summary>Any properties the API returned that this SDK does not model yet.</summary>
    [JsonExtensionData]
    public IDictionary<string, JsonElement>? AdditionalProperties { get; set; }
}
