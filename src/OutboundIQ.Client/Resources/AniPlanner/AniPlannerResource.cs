namespace OutboundIQ;

/// <inheritdoc cref="IAniPlannerResource"/>
internal sealed class AniPlannerResource(OutboundIQHttpClient http) : IAniPlannerResource
{
    private readonly OutboundIQHttpClient _http = http;

    public Task<AniPlannerGenerateResponse> GenerateAsync(
        AniPlannerGenerateRequest? request = null,
        CancellationToken cancellationToken = default)
    {
        // A null request still sends a body, serializing to exactly {} because every property is
        // optional and nulls are omitted. The API requires the body to be present.
        var body = OutboundIQHttpClient.Serialize(
            request ?? new AniPlannerGenerateRequest(),
            OutboundIQJsonContext.Default.AniPlannerGenerateRequest);

        return _http.SendAsync(
            ApiRequest.Post("/ani-planner/generate", body),
            OutboundIQJsonContext.Default.AniPlannerGenerateResponse,
            cancellationToken);
    }
}
