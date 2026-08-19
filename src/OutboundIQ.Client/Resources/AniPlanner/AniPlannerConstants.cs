namespace OutboundIQ;

/// <summary>
/// Target maximum dials per ANI per day, for
/// <see cref="AniPlannerGenerateRequest.DailyDialsTarget"/>.
/// </summary>
public static class AniPlannerDailyDialsTargets
{
    /// <summary>50 dials per ANI per day.</summary>
    public const string Best = "BEST";

    /// <summary>75 dials per ANI per day. The API default.</summary>
    public const string Better = "BETTER";

    /// <summary>100 dials per ANI per day.</summary>
    public const string Good = "GOOD";
}

/// <summary>
/// How dial volume is grouped into regions, for <see cref="AniPlannerGenerateRequest.GroupBy"/>.
/// </summary>
public static class AniPlannerGroupBys
{
    /// <summary>Group by the prospect's phone area code. The API default.</summary>
    public const string AreaCode = "area_code";

    /// <summary>Group by the area code of the ANI that placed the call.</summary>
    public const string Ani = "ani";

    /// <summary>Group by the area code of the prospect's ZIP. US and CA only.</summary>
    public const string Zip = "zip";
}

/// <summary>
/// Which ANIs count as current, for <see cref="AniPlannerGenerateRequest.InventoryMode"/>.
/// </summary>
public static class AniPlannerInventoryModes
{
    /// <summary>Count only active, managed numbers. The API default.</summary>
    public const string Managed = "managed";

    /// <summary>Count every number except those removed from the dialer.</summary>
    public const string All = "all";
}

/// <summary>
/// The countries a plan may be computed for, as reported by <see cref="AniPlannerPlan.Country"/>.
/// </summary>
public static class AniPlannerCountries
{
    /// <summary>United States.</summary>
    public const string Us = "us";

    /// <summary>Canada.</summary>
    public const string Ca = "ca";

    /// <summary>United Kingdom.</summary>
    public const string Uk = "uk";
}
