namespace OutboundIQ;

/// <summary>
/// Configuration for <see cref="OutboundIQClient"/>. Every value has a working default except
/// <see cref="ApiKey"/>, which falls back to the <c>OUTBOUNDIQ_API_KEY</c> environment variable.
/// </summary>
public sealed class OutboundIQClientOptions
{
    /// <summary>The default API base address, <c>https://api.outboundiq.cloud</c>.</summary>
    public static readonly Uri DefaultBaseUrl = new("https://api.outboundiq.cloud");

    /// <summary>The environment variable read when <see cref="ApiKey"/> is not set.</summary>
    public const string ApiKeyEnvironmentVariable = "OUTBOUNDIQ_API_KEY";

    /// <summary>
    /// A <c>universal</c> API key, generated in the outboundIQ workspace dashboard. When left
    /// <see langword="null"/>, the <c>OUTBOUNDIQ_API_KEY</c> environment variable is used instead.
    /// </summary>
    public string? ApiKey { get; set; }

    /// <summary>The API base address. Defaults to <c>https://api.outboundiq.cloud</c>.</summary>
    public Uri BaseUrl { get; set; } = DefaultBaseUrl;

    /// <summary>
    /// The timeout for a single attempt, not for the call as a whole. A request that is retried
    /// twice may therefore take up to three times this long, plus backoff. Defaults to 30 seconds.
    /// </summary>
    public TimeSpan Timeout { get; set; } = TimeSpan.FromSeconds(30);

    /// <summary>
    /// How many times to retry after the first attempt. The default of 2 allows 3 attempts in
    /// total. Set to 0 to disable retries.
    /// </summary>
    /// <remarks>
    /// GET requests are retried on network errors, 429, and 5xx responses. POST, PUT, and DELETE
    /// are retried only on 429, where the request is known not to have been processed. This avoids
    /// duplicating writes such as dial records when the outcome of a failed request is unknown.
    /// </remarks>
    public int MaxRetries { get; set; } = 2;

    /// <summary>
    /// The base delay for exponential backoff. Each retry waits
    /// <c>min(RetryBaseDelay * 2^attempt, 8s)</c> plus jitter. Defaults to 500 milliseconds.
    /// </summary>
    public TimeSpan RetryBaseDelay { get; set; } = TimeSpan.FromMilliseconds(500);

    /// <summary>
    /// Whether to read the <c>x-request-id</c> response header onto thrown exceptions. Defaults to
    /// <see langword="true"/>.
    /// </summary>
    public bool CaptureRequestId { get; set; } = true;

    /// <summary>
    /// Optional text appended to the <c>User-Agent</c> header, to identify your integration in
    /// outboundIQ's logs. For example <c>"acme-crm/2.1"</c>.
    /// </summary>
    public string? UserAgentSuffix { get; set; }

    internal void Validate()
    {
        if (BaseUrl is null)
        {
            throw new OutboundIQException($"{nameof(BaseUrl)} must not be null.");
        }

        if (!BaseUrl.IsAbsoluteUri)
        {
            throw new OutboundIQException($"{nameof(BaseUrl)} must be an absolute URI, but was '{BaseUrl}'.");
        }

        if (MaxRetries < 0)
        {
            throw new OutboundIQException($"{nameof(MaxRetries)} must not be negative, but was {MaxRetries}.");
        }

        if (Timeout <= TimeSpan.Zero)
        {
            throw new OutboundIQException($"{nameof(Timeout)} must be greater than zero, but was {Timeout}.");
        }

        if (RetryBaseDelay < TimeSpan.Zero)
        {
            throw new OutboundIQException($"{nameof(RetryBaseDelay)} must not be negative, but was {RetryBaseDelay}.");
        }
    }

    internal OutboundIQClientOptions Clone() => new()
    {
        ApiKey = ApiKey,
        BaseUrl = BaseUrl,
        Timeout = Timeout,
        MaxRetries = MaxRetries,
        RetryBaseDelay = RetryBaseDelay,
        CaptureRequestId = CaptureRequestId,
        UserAgentSuffix = UserAgentSuffix,
    };
}
