namespace OutboundIQ;

/// <inheritdoc cref="ICustomDialerResource"/>
internal sealed class CustomDialerResource : ICustomDialerResource
{
    internal CustomDialerResource(OutboundIQHttpClient http)
    {
        Campaigns = new CustomCampaigns(http);
        Dispos = new CustomDispos(http);
        Anis = new CustomAnis(http);
    }

    public ICustomCampaigns Campaigns { get; }

    public ICustomDispos Dispos { get; }

    public ICustomAnis Anis { get; }
}

/// <inheritdoc cref="ICustomCampaigns"/>
internal sealed class CustomCampaigns(OutboundIQHttpClient http) : ICustomCampaigns
{
    private const string Path = "/custom/campaigns";
    private readonly OutboundIQHttpClient _http = http;

    public Task<CustomApiResponse> CreateAsync(CustomCampaign campaign, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(campaign);

        return _http.SendAsync(
            ApiRequest.Post(Path, OutboundIQHttpClient.Serialize(campaign, OutboundIQJsonContext.Default.CustomCampaign)),
            OutboundIQJsonContext.Default.CustomApiResponse,
            cancellationToken);
    }

    public Task<CustomApiResponse> UpdateAsync(CustomCampaignUpdate campaign, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(campaign);

        return _http.SendAsync(
            ApiRequest.Put(Path, OutboundIQHttpClient.Serialize(campaign, OutboundIQJsonContext.Default.CustomCampaignUpdate)),
            OutboundIQJsonContext.Default.CustomApiResponse,
            cancellationToken);
    }

    public Task<CustomApiResponse> GetAsync(string id, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrEmpty(id);

        return _http.SendAsync(
            ApiRequest.Get(Path, [new("id", id)]),
            OutboundIQJsonContext.Default.CustomApiResponse,
            cancellationToken);
    }

    public Task<CustomApiResponse> DeleteAsync(string id, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrEmpty(id);

        return _http.SendAsync(
            ApiRequest.Delete(Path, OutboundIQHttpClient.Serialize(new CustomIdBody { Id = id }, OutboundIQJsonContext.Default.CustomIdBody)),
            OutboundIQJsonContext.Default.CustomApiResponse,
            cancellationToken);
    }
}

/// <inheritdoc cref="ICustomDispos"/>
internal sealed class CustomDispos(OutboundIQHttpClient http) : ICustomDispos
{
    private const string Path = "/custom/dispos";
    private readonly OutboundIQHttpClient _http = http;

    public Task<CustomApiResponse> CreateAsync(CustomDispo dispo, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(dispo);

        return _http.SendAsync(
            ApiRequest.Post(Path, OutboundIQHttpClient.Serialize(dispo, OutboundIQJsonContext.Default.CustomDispo)),
            OutboundIQJsonContext.Default.CustomApiResponse,
            cancellationToken);
    }

    public Task<CustomApiResponse> UpdateAsync(CustomDispoUpdate dispo, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(dispo);

        return _http.SendAsync(
            ApiRequest.Put(Path, OutboundIQHttpClient.Serialize(dispo, OutboundIQJsonContext.Default.CustomDispoUpdate)),
            OutboundIQJsonContext.Default.CustomApiResponse,
            cancellationToken);
    }

    public Task<CustomApiResponse> GetAsync(string id, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrEmpty(id);

        return _http.SendAsync(
            ApiRequest.Get(Path, [new("id", id)]),
            OutboundIQJsonContext.Default.CustomApiResponse,
            cancellationToken);
    }

    public Task<CustomApiResponse> DeleteAsync(string id, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrEmpty(id);

        return _http.SendAsync(
            ApiRequest.Delete(Path, OutboundIQHttpClient.Serialize(new CustomIdBody { Id = id }, OutboundIQJsonContext.Default.CustomIdBody)),
            OutboundIQJsonContext.Default.CustomApiResponse,
            cancellationToken);
    }
}

/// <inheritdoc cref="ICustomAnis"/>
internal sealed class CustomAnis(OutboundIQHttpClient http) : ICustomAnis
{
    private const string Path = "/custom/anis";
    private readonly OutboundIQHttpClient _http = http;

    public Task<CustomApiResponse> CreateAsync(CustomAni ani, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(ani);

        return _http.SendAsync(
            ApiRequest.Post(Path, OutboundIQHttpClient.Serialize(ani, OutboundIQJsonContext.Default.CustomAni)),
            OutboundIQJsonContext.Default.CustomApiResponse,
            cancellationToken);
    }

    public Task<CustomApiResponse> UpdateAsync(CustomAniUpdate ani, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(ani);

        return _http.SendAsync(
            ApiRequest.Put(Path, OutboundIQHttpClient.Serialize(ani, OutboundIQJsonContext.Default.CustomAniUpdate)),
            OutboundIQJsonContext.Default.CustomApiResponse,
            cancellationToken);
    }

    public Task<GetCustomAniResponse> GetAsync(string number, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrEmpty(number);

        return _http.SendAsync(
            ApiRequest.Get(Path, [new("number", number)]),
            OutboundIQJsonContext.Default.GetCustomAniResponse,
            cancellationToken);
    }

    public Task<CustomApiResponse> DeleteAsync(string number, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrEmpty(number);

        return _http.SendAsync(
            ApiRequest.Delete(Path, OutboundIQHttpClient.Serialize(new CustomNumberBody { Number = number }, OutboundIQJsonContext.Default.CustomNumberBody)),
            OutboundIQJsonContext.Default.CustomApiResponse,
            cancellationToken);
    }
}
