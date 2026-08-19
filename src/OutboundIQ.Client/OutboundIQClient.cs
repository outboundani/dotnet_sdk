using System.Net.Http;

namespace OutboundIQ;

/// <summary>
/// The outboundIQ API client.
/// </summary>
/// <remarks>
/// <para>
/// The client is stateless once constructed and is safe to share across threads. Create one and
/// keep it for the lifetime of your application — registering it as a singleton is the intended
/// usage. Creating one per request will exhaust sockets.
/// </para>
/// <para>
/// Requests carry a per-attempt timeout and are retried with exponential backoff. GET is retried
/// on network errors, 429, and 5xx. POST, PUT, and DELETE are retried only on 429, where the
/// request is known not to have been processed, so a dial is never recorded twice.
/// </para>
/// </remarks>
/// <example>
/// <code>
/// using var client = new OutboundIQClient("oiq_...");
///
/// var result = await client.Assignment.NextAsync(new AssignmentRequest
/// {
///     ProspectPhone = "5559876543",
///     ProspectZip = "90210",
/// });
///
/// if (result.Success)
/// {
///     Console.WriteLine($"Dial from {result.Ani}");
/// }
/// </code>
/// </example>
public sealed class OutboundIQClient : IOutboundIQClient
{
    private readonly OutboundIQHttpClient _http;

    /// <summary>
    /// Creates a client that reads its API key from the <c>OUTBOUNDIQ_API_KEY</c> environment
    /// variable.
    /// </summary>
    /// <exception cref="OutboundIQException">The environment variable is not set.</exception>
    public OutboundIQClient()
        : this(new OutboundIQClientOptions())
    {
    }

    /// <summary>
    /// Creates a client with an explicit API key.
    /// </summary>
    /// <param name="apiKey">
    /// A <c>universal</c> API key, generated in the outboundIQ workspace dashboard.
    /// </param>
    public OutboundIQClient(string apiKey)
        : this(new OutboundIQClientOptions { ApiKey = apiKey })
    {
    }

    /// <summary>
    /// Creates a client with full configuration.
    /// </summary>
    /// <param name="options">The configuration to use.</param>
    /// <exception cref="OutboundIQException">
    /// No API key was supplied or found in the environment, or an option is out of range.
    /// </exception>
    public OutboundIQClient(OutboundIQClientOptions options)
        : this(options, httpClient: null)
    {
    }

    /// <summary>
    /// Creates a client that sends through a caller-supplied <see cref="HttpClient"/>, for use
    /// with <c>IHttpClientFactory</c> or a custom handler pipeline.
    /// </summary>
    /// <param name="options">The configuration to use.</param>
    /// <param name="httpClient">
    /// The transport. It is <em>not</em> disposed by this client. Set its
    /// <see cref="HttpClient.Timeout"/> to <see cref="System.Threading.Timeout.InfiniteTimeSpan"/>
    /// if you want <see cref="OutboundIQClientOptions.Timeout"/> to be the only timeout in effect;
    /// otherwise the shorter of the two wins.
    /// </param>
    public OutboundIQClient(OutboundIQClientOptions options, HttpClient? httpClient)
    {
        ArgumentNullException.ThrowIfNull(options);

        _http = new OutboundIQHttpClient(options, httpClient);

        Assignment = new AssignmentResource(_http);
        Dials = new DialsResource(_http);
        Custom = new CustomDialerResource(_http);
        Nrm = new NrmResource(_http);
        LiveFeed = new LiveFeedResource(_http);
        AniPlanner = new AniPlannerResource(_http);
    }

    /// <summary>
    /// The version of this SDK, as sent in the <c>User-Agent</c> header.
    /// </summary>
    public static string Version => OutboundIQVersion.Value;

    /// <inheritdoc/>
    public IAssignmentResource Assignment { get; }

    /// <inheritdoc/>
    public IDialsResource Dials { get; }

    /// <inheritdoc/>
    public ICustomDialerResource Custom { get; }

    /// <inheritdoc/>
    public INrmResource Nrm { get; }

    /// <inheritdoc/>
    public ILiveFeedResource LiveFeed { get; }

    /// <inheritdoc/>
    public IAniPlannerResource AniPlanner { get; }

    /// <summary>
    /// Releases the underlying transport, unless it was supplied by the caller.
    /// </summary>
    public void Dispose() => _http.Dispose();
}
