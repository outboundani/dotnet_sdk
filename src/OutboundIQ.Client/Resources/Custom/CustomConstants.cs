namespace OutboundIQ;

/// <summary>
/// The values accepted for <see cref="CustomCampaign.Type"/>.
/// </summary>
public static class CustomCampaignTypes
{
    /// <summary>An inbound campaign.</summary>
    public const string Inbound = "Inbound";

    /// <summary>An outbound campaign.</summary>
    public const string Outbound = "Outbound";

    /// <summary>A campaign handling both directions.</summary>
    public const string Blended = "Blended";
}

/// <summary>
/// The values accepted for <see cref="CustomDispo.Type"/>.
/// </summary>
public static class CustomDispoTypes
{
    /// <summary>Applied automatically by the dialer.</summary>
    public const string System = "System";

    /// <summary>Selected by an agent.</summary>
    public const string Agent = "Agent";
}

/// <summary>
/// The values accepted for <see cref="CustomAni.CountryCode"/>.
/// </summary>
/// <remarks>
/// These are constants rather than an enum because <c>"+1"</c> and <c>"+44"</c> are not valid C#
/// identifiers, and mapping them would need a hand-written converter on every target framework.
/// </remarks>
public static class CountryCodes
{
    /// <summary>United States and Canada.</summary>
    public const string Us = "+1";

    /// <summary>United Kingdom.</summary>
    public const string Uk = "+44";
}
