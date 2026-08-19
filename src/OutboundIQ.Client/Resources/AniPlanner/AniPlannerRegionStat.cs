using System.Text.Json.Serialization;

namespace OutboundIQ;

/// <summary>
/// One region row of a plan, sorted by <see cref="DailyDialsAverage"/> descending.
/// </summary>
public sealed record AniPlannerRegionStat
{
    /// <summary>
    /// Region or city name for US and CA, or the comma-joined counties served by the area code
    /// for the UK.
    /// </summary>
    [JsonPropertyName("region")]
    public string? Region { get; init; }

    /// <summary>
    /// State abbreviation for US and CA, <c>"TF"</c> for toll-free, or the area code for the UK.
    /// </summary>
    [JsonPropertyName("state")]
    public string? State { get; init; }

    /// <summary>The area codes rolled up into this region.</summary>
    [JsonPropertyName("areaCodes")]
    public IReadOnlyList<string> AreaCodes { get; init; } = [];

    /// <summary>Total dials in the region divided by business days, rounded up.</summary>
    [JsonPropertyName("dailyDialsAverage")]
    public double DailyDialsAverage { get; init; }

    /// <summary>ANIs currently held for this region, per the requested inventory mode.</summary>
    [JsonPropertyName("currentAnis")]
    public double CurrentAnis { get; init; }

    /// <summary>
    /// Recommended ANI count, with a per-region minimum floor. Zero when
    /// <see cref="BelowThreshold"/> is <see langword="true"/>.
    /// </summary>
    [JsonPropertyName("proposedAnis")]
    public double ProposedAnis { get; init; }

    /// <summary>
    /// <see cref="ProposedAnis"/> minus <see cref="CurrentAnis"/>. Positive means add numbers,
    /// negative means the region is over-provisioned.
    /// </summary>
    [JsonPropertyName("difference")]
    public double Difference { get; init; }

    /// <summary>
    /// Whether the region averages fewer than 20 dials per business day. Such regions are
    /// recommended zero ANIs and excluded from the proposed total, but are still returned so
    /// numbers already provisioned there show up as surplus.
    /// </summary>
    [JsonPropertyName("belowThreshold")]
    public bool BelowThreshold { get; init; }
}
