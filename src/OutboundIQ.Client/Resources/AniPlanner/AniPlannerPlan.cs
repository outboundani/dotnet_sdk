using System.Text.Json;
using System.Text.Json.Serialization;

namespace OutboundIQ;

/// <summary>
/// The ANI provisioning recommendation returned by <c>POST /ani-planner/generate</c>.
/// </summary>
public sealed record AniPlannerPlan
{
    /// <summary>
    /// The country the plan was computed for, detected from the company's dialer. Determines how
    /// regions are grouped. See <see cref="AniPlannerCountries"/>.
    /// </summary>
    [JsonPropertyName("country")]
    public string? Country { get; init; }

    /// <summary>The effective start of the analyzed range, after defaults were applied.</summary>
    [JsonPropertyName("dateStart")]
    public string? DateStart { get; init; }

    /// <summary>The effective end of the analyzed range, after defaults were applied.</summary>
    [JsonPropertyName("dateEnd")]
    public string? DateEnd { get; init; }

    /// <summary>Weekdays in the range. Daily averages are computed against this.</summary>
    [JsonPropertyName("bizDays")]
    public double BizDays { get; init; }

    /// <summary>The resolved numeric target: 50, 75, or 100.</summary>
    [JsonPropertyName("dailyDialsTarget")]
    public double DailyDialsTarget { get; init; }

    /// <summary>How dial volume was grouped. See <see cref="AniPlannerGroupBys"/>.</summary>
    [JsonPropertyName("groupBy")]
    public string? GroupBy { get; init; }

    /// <summary>Which ANIs counted as current. See <see cref="AniPlannerInventoryModes"/>.</summary>
    [JsonPropertyName("inventoryMode")]
    public string? InventoryMode { get; init; }

    /// <summary>Per-region recommendations, highest volume first.</summary>
    [JsonPropertyName("regionStats")]
    public IReadOnlyList<AniPlannerRegionStat> RegionStats { get; init; } = [];

    /// <summary>Total dials across the analyzed range.</summary>
    [JsonPropertyName("totalDials")]
    public double TotalDials { get; init; }

    /// <summary>Total contacts across the analyzed range.</summary>
    [JsonPropertyName("totalContacts")]
    public double TotalContacts { get; init; }

    /// <summary>Contact rate across all regions, as a percentage.</summary>
    [JsonPropertyName("overallContactRate")]
    public double OverallContactRate { get; init; }

    /// <summary>ANIs held across every region, including those below the volume threshold.</summary>
    [JsonPropertyName("totalCurrentAnis")]
    public double TotalCurrentAnis { get; init; }

    /// <summary>Recommended ANIs, summing only regions above the volume threshold.</summary>
    [JsonPropertyName("totalProposedAnis")]
    public double TotalProposedAnis { get; init; }

    /// <summary>
    /// <see cref="TotalProposedAnis"/> minus <see cref="TotalCurrentAnis"/>, so surplus held in
    /// low-volume regions is reflected.
    /// </summary>
    [JsonPropertyName("aniDifference")]
    public double AniDifference { get; init; }

    /// <summary>
    /// True only when grouping by ZIP but no ZIP area-code data was found despite dials existing.
    /// </summary>
    [JsonPropertyName("missingZipData")]
    public bool MissingZipData { get; init; }

    /// <summary>
    /// Recommended toll-free ANI count. Zero for US and CA, where toll-free appears as a
    /// <c>"TF"</c> row in <see cref="RegionStats"/> instead.
    /// </summary>
    [JsonPropertyName("tollFreeRecommendation")]
    public double TollFreeRecommendation { get; init; }

    /// <summary>Numeric identifiers of the outbound campaigns included in the analysis.</summary>
    [JsonPropertyName("selectedCampaigns")]
    public IReadOnlyList<long> SelectedCampaigns { get; init; } = [];

    /// <summary>The company's outbound campaigns and their inbound mappings.</summary>
    [JsonPropertyName("campaigns")]
    public IReadOnlyList<AniPlannerCampaign> Campaigns { get; init; } = [];

    /// <summary>Any properties the API returned that this SDK does not model yet.</summary>
    [JsonExtensionData]
    public IDictionary<string, JsonElement>? AdditionalProperties { get; set; }
}
