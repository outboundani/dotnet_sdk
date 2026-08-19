namespace OutboundIQ;

/// <inheritdoc cref="IAssignmentResource"/>
internal sealed class AssignmentResource(OutboundIQHttpClient http) : IAssignmentResource
{
    private readonly OutboundIQHttpClient _http = http;

    public Task<AssignmentResponse> NextAsync(AssignmentRequest request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        return _http.SendAsync(
            ApiRequest.Post("/assignment", OutboundIQHttpClient.Serialize(request, OutboundIQJsonContext.Default.AssignmentRequest)),
            OutboundIQJsonContext.Default.AssignmentResponse,
            cancellationToken);
    }

    public Task<AssignmentBatchResponse> BatchAsync(AssignmentBatchRequest request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        return _http.SendAsync(
            ApiRequest.Post("/assignment/batch", OutboundIQHttpClient.Serialize(request, OutboundIQJsonContext.Default.AssignmentBatchRequest)),
            OutboundIQJsonContext.Default.AssignmentBatchResponse,
            cancellationToken);
    }
}
