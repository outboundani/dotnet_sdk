namespace OutboundIQ;

/// <inheritdoc cref="ILiveFeedResource"/>
internal sealed class LiveFeedResource : ILiveFeedResource
{
    internal LiveFeedResource(OutboundIQHttpClient http) => RingCx = new RingCxLiveFeed(http);

    public IRingCxLiveFeed RingCx { get; }
}

/// <inheritdoc cref="IRingCxLiveFeed"/>
internal sealed class RingCxLiveFeed(OutboundIQHttpClient http) : IRingCxLiveFeed
{
    private readonly OutboundIQHttpClient _http = http;

    public Task<RingCxUploadResponse> UploadAsync(RingCxUploadRequest request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        return _http.SendAsync(
            ApiRequest.Post("/live-feed/ringcx", OutboundIQHttpClient.Serialize(request, OutboundIQJsonContext.Default.RingCxUploadRequest)),
            OutboundIQJsonContext.Default.RingCxUploadResponse,
            cancellationToken);
    }
}
