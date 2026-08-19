namespace OutboundIQ;

/// <summary>
/// ANI Planner: analyzes recent dial volume against current ANI inventory and recommends how many
/// numbers to provision in each region. See
/// <see href="https://docs.outboundiq.cloud/reference/ani-planner/"/>.
/// </summary>
public interface IAniPlannerResource
{
    /// <summary>
    /// Computes the ANI recommendation for the authenticated company over a date range.
    /// <c>POST /ani-planner/generate</c>
    /// </summary>
    /// <param name="request">
    /// Optional filters and targets. Pass <see langword="null"/> to accept every default.
    /// </param>
    /// <param name="cancellationToken">Cancels the call.</param>
    Task<AniPlannerGenerateResponse> GenerateAsync(
        AniPlannerGenerateRequest? request = null,
        CancellationToken cancellationToken = default);
}
