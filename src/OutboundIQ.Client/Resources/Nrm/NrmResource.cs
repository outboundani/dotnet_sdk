using System.Globalization;

namespace OutboundIQ;

/// <inheritdoc cref="INrmResource"/>
internal sealed class NrmResource(OutboundIQHttpClient http) : INrmResource
{
    private readonly OutboundIQHttpClient _http = http;

    public Task<NrmListAnisResponse> ListAnisAsync(
        NrmListAnisParams? parameters = null,
        CancellationToken cancellationToken = default)
    {
        // Unset parameters are omitted from the query string entirely, so the API applies its own
        // defaults rather than receiving an explicit null.
        var query = new List<KeyValuePair<string, string?>>(3)
        {
            new("page", parameters?.Page?.ToString(CultureInfo.InvariantCulture)),
            new("page_size", parameters?.PageSize?.ToString(CultureInfo.InvariantCulture)),
            new("number", parameters?.Number),
        };

        return _http.SendAsync(
            ApiRequest.Get("/nrm/anis", query),
            OutboundIQJsonContext.Default.NrmListAnisResponse,
            cancellationToken);
    }

    public Task<NrmActionResponse> RemediateAsync(NrmRemediateRequest request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        return _http.SendAsync(
            ApiRequest.Post("/nrm/remediate", OutboundIQHttpClient.Serialize(request, OutboundIQJsonContext.Default.NrmRemediateRequest)),
            OutboundIQJsonContext.Default.NrmActionResponse,
            cancellationToken);
    }

    public Task<NrmActionResponse> PauseAsync(NrmPauseRequest request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        return _http.SendAsync(
            ApiRequest.Post("/nrm/pause", OutboundIQHttpClient.Serialize(request, OutboundIQJsonContext.Default.NrmPauseRequest)),
            OutboundIQJsonContext.Default.NrmActionResponse,
            cancellationToken);
    }

    public Task<NrmActionResponse> ActivateAsync(NrmActivateRequest request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        return _http.SendAsync(
            ApiRequest.Post("/nrm/activate", OutboundIQHttpClient.Serialize(request, OutboundIQJsonContext.Default.NrmActivateRequest)),
            OutboundIQJsonContext.Default.NrmActionResponse,
            cancellationToken);
    }
}
