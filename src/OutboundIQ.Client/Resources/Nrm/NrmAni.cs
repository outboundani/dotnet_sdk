using System.Text.Json;
using System.Text.Json.Serialization;

namespace OutboundIQ;

/// <summary>
/// An ANI as returned by the NRM inventory, with 30-day reputation statistics.
/// </summary>
public sealed record NrmAni
{
    /// <summary>The ANI identifier. The API returns this as either a number or a string.</summary>
    [JsonPropertyName("id")]
    public NumberOrString Id { get; init; }

    /// <summary>The phone number.</summary>
    [JsonPropertyName("phone")]
    public string? Phone { get; init; }

    /// <summary>The brand displayed on branded calls.</summary>
    [JsonPropertyName("brand")]
    public string? Brand { get; init; }

    /// <summary>The status code. The API returns this as either a number or a string.</summary>
    [JsonPropertyName("status")]
    public NumberOrString Status { get; init; }

    /// <summary>A human-readable status.</summary>
    [JsonPropertyName("statusLabel")]
    public string? StatusLabel { get; init; }

    /// <summary>The campaign this number is assigned to.</summary>
    [JsonPropertyName("campaignName")]
    public string? CampaignName { get; init; }

    /// <summary>The number's area code.</summary>
    [JsonPropertyName("areaCode")]
    public string? AreaCode { get; init; }

    /// <summary>The region the area code belongs to.</summary>
    [JsonPropertyName("region")]
    public string? Region { get; init; }

    /// <summary>The state or province.</summary>
    [JsonPropertyName("state")]
    public string? State { get; init; }

    /// <summary>The locality.</summary>
    [JsonPropertyName("locality")]
    public string? Locality { get; init; }

    /// <summary>How many times the number has been assigned.</summary>
    [JsonPropertyName("frequencyAssigned")]
    public double? FrequencyAssigned { get; init; }

    /// <summary>How many times the number has been dialed from.</summary>
    [JsonPropertyName("frequencyDialed")]
    public double? FrequencyDialed { get; init; }

    /// <summary>How many contacts the number has produced.</summary>
    [JsonPropertyName("frequencyContacted")]
    public double? FrequencyContacted { get; init; }

    /// <summary>When the number was last dialed from.</summary>
    [JsonPropertyName("dateLastDialed")]
    public string? DateLastDialed { get; init; }

    /// <summary>When the number was activated.</summary>
    [JsonPropertyName("dateActivated")]
    public string? DateActivated { get; init; }

    /// <summary>When the number was deactivated.</summary>
    [JsonPropertyName("dateDeactivated")]
    public string? DateDeactivated { get; init; }

    /// <summary>When the number was last inventoried.</summary>
    [JsonPropertyName("dateInventoried")]
    public string? DateInventoried { get; init; }

    /// <summary>When the record was created.</summary>
    [JsonPropertyName("createdAt")]
    public string? CreatedAt { get; init; }

    /// <summary>Dials placed from this number in the last 30 days.</summary>
    [JsonPropertyName("last30DaysDials")]
    public double? Last30DaysDials { get; init; }

    /// <summary>Contacts made from this number in the last 30 days.</summary>
    [JsonPropertyName("last30DaysContacts")]
    public double? Last30DaysContacts { get; init; }

    /// <summary>Contact rate as a percentage.</summary>
    [JsonPropertyName("contactRate")]
    public double? ContactRate { get; init; }

    /// <summary>Success rate as a percentage.</summary>
    [JsonPropertyName("successRate")]
    public double? SuccessRate { get; init; }

    /// <summary>Block rate as a percentage.</summary>
    [JsonPropertyName("blockRate")]
    public double? BlockRate { get; init; }

    /// <summary>No-answer rate as a percentage.</summary>
    [JsonPropertyName("noAnswerRate")]
    public double? NoAnswerRate { get; init; }

    /// <summary>Any properties the API returned that this SDK does not model yet.</summary>
    [JsonExtensionData]
    public IDictionary<string, JsonElement>? AdditionalProperties { get; set; }
}
