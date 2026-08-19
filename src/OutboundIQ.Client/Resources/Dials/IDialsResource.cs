namespace OutboundIQ;

/// <summary>
/// Dial ingestion. See <see href="https://docs.outboundiq.cloud/reference/dials/"/>.
/// </summary>
public interface IDialsResource
{
    /// <summary>
    /// Posts one dial record for processing and enrichment. <c>POST /dials</c>
    /// </summary>
    /// <param name="dial">The dial record.</param>
    /// <param name="cancellationToken">Cancels the call.</param>
    /// <returns>An acknowledgement that the dial was queued.</returns>
    /// <remarks>
    /// This call is not retried on network failures or 5xx responses, because a dial that may
    /// already have been recorded must not be recorded twice. Set
    /// <see cref="DialRecord.DialId"/> if you want to retry safely yourself.
    /// </remarks>
    Task<DialQueuedResponse> CreateAsync(DialRecord dial, CancellationToken cancellationToken = default);
}
