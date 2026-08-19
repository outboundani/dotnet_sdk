namespace OutboundIQ;

/// <summary>
/// Number reputation management. See <see href="https://docs.outboundiq.cloud/reference/nrm/v2/"/>.
/// </summary>
public interface INrmResource
{
    /// <summary>
    /// Lists the ANI inventory with reputation and performance statistics.
    /// <c>GET /nrm/anis</c>
    /// </summary>
    /// <param name="parameters">Paging and filtering. Defaults to page 1 when omitted.</param>
    /// <param name="cancellationToken">Cancels the call.</param>
    Task<NrmListAnisResponse> ListAnisAsync(NrmListAnisParams? parameters = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Requests remediation for a flagged or blocked ANI. <c>POST /nrm/remediate</c>
    /// </summary>
    /// <param name="request">The ANI to remediate.</param>
    /// <param name="cancellationToken">Cancels the call.</param>
    /// <remarks>
    /// Remediation has a 30-day cooldown per ANI. A request inside that window returns HTTP 200
    /// with <c>status</c> set to <c>"within cooldown"</c> rather than an error.
    /// </remarks>
    Task<NrmActionResponse> RemediateAsync(NrmRemediateRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Puts an ANI into a cooling-off period. <c>POST /nrm/pause</c>
    /// </summary>
    /// <param name="request">The ANI to pause.</param>
    /// <param name="cancellationToken">Cancels the call.</param>
    Task<NrmActionResponse> PauseAsync(NrmPauseRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Returns a paused ANI to active dialing. <c>POST /nrm/activate</c>
    /// </summary>
    /// <param name="request">The ANI to reactivate, and the date to do so.</param>
    /// <param name="cancellationToken">Cancels the call.</param>
    Task<NrmActionResponse> ActivateAsync(NrmActivateRequest request, CancellationToken cancellationToken = default);
}
