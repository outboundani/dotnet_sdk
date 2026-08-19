namespace OutboundIQ;

/// <summary>
/// The outboundIQ API surface, grouped by resource.
/// </summary>
/// <remarks>
/// Depend on this interface rather than <see cref="OutboundIQClient"/> where you want to
/// substitute a fake in tests.
/// </remarks>
public interface IOutboundIQClient : IDisposable
{
    /// <summary>ANI assignment.</summary>
    IAssignmentResource Assignment { get; }

    /// <summary>Dial ingestion.</summary>
    IDialsResource Dials { get; }

    /// <summary>Custom dialer integration: campaigns, dispositions, and ANIs.</summary>
    ICustomDialerResource Custom { get; }

    /// <summary>Number reputation management.</summary>
    INrmResource Nrm { get; }

    /// <summary>Live lead feeds into supported dialers.</summary>
    ILiveFeedResource LiveFeed { get; }

    /// <summary>ANI provisioning recommendations.</summary>
    IAniPlannerResource AniPlanner { get; }
}
