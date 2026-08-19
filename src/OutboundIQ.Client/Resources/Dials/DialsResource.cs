namespace OutboundIQ;

/// <inheritdoc cref="IDialsResource"/>
internal sealed class DialsResource(OutboundIQHttpClient http) : IDialsResource
{
    private readonly OutboundIQHttpClient _http = http;

    public Task<DialQueuedResponse> CreateAsync(DialRecord dial, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(dial);

        return _http.SendAsync(
            ApiRequest.Post("/dials", OutboundIQHttpClient.Serialize(dial, OutboundIQJsonContext.Default.DialRecord)),
            OutboundIQJsonContext.Default.DialQueuedResponse,
            cancellationToken);
    }
}
