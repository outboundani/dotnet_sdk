namespace OutboundIQ;

/// <summary>
/// ANI assignment. See <see href="https://docs.outboundiq.cloud/reference/assignment/"/>.
/// </summary>
public interface IAssignmentResource
{
    /// <summary>
    /// Gets the next ANI to dial from for a single prospect. <c>POST /assignment</c>
    /// </summary>
    /// <param name="request">The prospect to assign an ANI for.</param>
    /// <param name="cancellationToken">Cancels the call.</param>
    /// <returns>The assignment outcome.</returns>
    Task<AssignmentResponse> NextAsync(AssignmentRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets ANIs for a list of leads in one call. <c>POST /assignment/batch</c>
    /// </summary>
    /// <param name="request">The leads to assign ANIs for.</param>
    /// <param name="cancellationToken">Cancels the call.</param>
    /// <returns>One result per lead. Individual leads may fail while the batch succeeds.</returns>
    Task<AssignmentBatchResponse> BatchAsync(AssignmentBatchRequest request, CancellationToken cancellationToken = default);
}
