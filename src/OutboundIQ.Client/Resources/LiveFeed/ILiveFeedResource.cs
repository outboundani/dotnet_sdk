namespace OutboundIQ;

/// <summary>
/// Live lead feeds into supported dialers.
/// See <see href="https://docs.outboundiq.cloud/reference/live-feed/ringcx/"/>.
/// </summary>
public interface ILiveFeedResource
{
    /// <summary>The RingCX feed.</summary>
    IRingCxLiveFeed RingCx { get; }
}

/// <summary>
/// The RingCX live lead feed.
/// </summary>
public interface IRingCxLiveFeed
{
    /// <summary>
    /// Pushes a single lead into a running RingCX campaign. <c>POST /live-feed/ringcx</c>
    /// </summary>
    /// <param name="request">The campaign, lead, and optional dialer behavior.</param>
    /// <param name="cancellationToken">Cancels the call.</param>
    /// <remarks>
    /// This sends JSON, not a file. The caller ID is assigned by outboundIQ.
    /// </remarks>
    Task<RingCxUploadResponse> UploadAsync(RingCxUploadRequest request, CancellationToken cancellationToken = default);
}
