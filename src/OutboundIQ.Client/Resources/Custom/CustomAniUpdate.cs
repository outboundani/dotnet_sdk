using System.Text.Json.Serialization;

namespace OutboundIQ;

/// <summary>
/// A partial ANI update for <c>PUT /custom/anis</c>. Omitted properties are left unchanged.
/// </summary>
public sealed record CustomAniUpdate
{
    /// <summary>The phone number identifying the ANI to update.</summary>
    [JsonPropertyName("number")]
    public required string Number { get; init; }

    /// <summary>The new country calling code. See <see cref="CountryCodes"/>.</summary>
    [JsonPropertyName("country_code")]
    public string? CountryCode { get; init; }

    /// <summary>The new inbound group.</summary>
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
}
