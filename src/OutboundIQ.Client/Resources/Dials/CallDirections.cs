namespace OutboundIQ;

/// <summary>
/// The documented values for <see cref="DialRecord.CallDirection"/>.
/// </summary>
/// <remarks>
/// The field is a plain <see cref="string"/> rather than an enum because the platform may add
/// directions before this SDK does. An unrecognized value round-trips instead of throwing.
/// </remarks>
public static class CallDirections
{
    /// <summary>An inbound call.</summary>
    public const string Inbound = "Inbound";

    /// <summary>An outbound call.</summary>
    public const string Outbound = "Outbound";

    /// <summary>An SMS message.</summary>
    public const string Sms = "SMS";

    /// <summary>A preview-dialed call.</summary>
    public const string Preview = "Preview";

    /// <summary>A transferred call.</summary>
    public const string Transfer = "Transfer";

    /// <summary>A manually dialed call.</summary>
    public const string Manual = "Manual";
}
