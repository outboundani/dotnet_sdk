using System.Text.Json.Serialization;

namespace OutboundIQ;

/// <summary>
/// An ANI to create through <c>POST /custom/anis</c>.
/// </summary>
public sealed record CustomAni
{
    /// <summary>The country calling code. See <see cref="CountryCodes"/>.</summary>
    [JsonPropertyName("country_code")]
    public required string CountryCode { get; init; }

    /// <summary>
    /// The phone number. Formatting characters are stripped by the API before storage.
    /// </summary>
    [JsonPropertyName("number")]
    public required string Number { get; init; }

    /// <summary>The inbound group this number routes to.</summary>
    [JsonPropertyName("inbound_group_id")]
    public required string InboundGroupId { get; init; }

    /// <summary>Whether the number carries branded caller ID.</summary>
    [JsonPropertyName("is_branded")]
    public bool? IsBranded { get; init; }

    /// <summary>The brand displayed on branded calls.</summary>
    [JsonPropertyName("brand_name")]
    public string? BrandName { get; init; }

    /// <summary>
    /// Six digit carrier identifier. The full list is published at
    /// <see href="https://docs.outboundiq.cloud/reference/nrm/carriers/"/>.
    /// </summary>
    [JsonPropertyName("carrier_id")]
    public string? CarrierId { get; init; }
}
