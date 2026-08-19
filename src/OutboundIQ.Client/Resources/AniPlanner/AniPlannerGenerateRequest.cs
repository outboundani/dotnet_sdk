using System.Text.Json.Serialization;

namespace OutboundIQ;

/// <summary>
/// Request body for <c>POST /ani-planner/generate</c>. Every property is optional.
/// </summary>
public sealed record AniPlannerGenerateRequest
{
    /// <summary>
    /// Start of the range to analyze, as <c>"YYYY-MM-DD"</c>. Defaults to the start of the
    /// current month.
    /// </summary>
    [JsonPropertyName("dateStart")]
    public string? DateStart { get; init; }

    /// <summary>End of the range to analyze, as <c>"YYYY-MM-DD"</c>. Defaults to yesterday.</summary>
    [JsonPropertyName("dateEnd")]
    public string? DateEnd { get; init; }

    /// <summary>
    /// Target maximum dials per ANI per day. See <see cref="AniPlannerDailyDialsTargets"/>.
    /// Defaults to <c>BETTER</c>.
    /// </summary>
    [JsonPropertyName("dailyDialsTarget")]
    public string? DailyDialsTarget { get; init; }

    /// <summary>
    /// How dial volume is grouped. See <see cref="AniPlannerGroupBys"/>. Defaults to
    /// <c>area_code</c>.
    /// </summary>
    [JsonPropertyName("groupBy")]
    public string? GroupBy { get; init; }

    /// <summary>
    /// Which ANIs count as current. See <see cref="AniPlannerInventoryModes"/>. Defaults to
    /// <c>managed</c>.
    /// </summary>
    [JsonPropertyName("inventoryMode")]
    public string? InventoryMode { get; init; }

    /// <summary>
    /// Include only these outbound campaigns, by internal identifier. Provide at most one of this
    /// and <see cref="CampaignsExclude"/>. Omit both to include every campaign. Unknown
    /// identifiers produce a 400.
    /// </summary>
    [JsonPropertyName("campaigns")]
    public IReadOnlyList<string>? Campaigns { get; init; }

    /// <summary>Include every campaign except these, by internal identifier.</summary>
    [JsonPropertyName("campaignsExclude")]
    public IReadOnlyList<string>? CampaignsExclude { get; init; }
}
