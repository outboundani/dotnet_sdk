using System.Globalization;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Text.Json.Serialization.Metadata;

namespace OutboundIQ;

/// <summary>
/// The transport shared by every resource: authentication, retries, timeouts, and error mapping.
/// </summary>
/// <remarks>
/// This mirrors the TypeScript SDK's core HTTP client so the two behave identically. In
/// particular: only GET is treated as retry-safe, the timeout applies per attempt rather than to
/// the call as a whole, and <c>Retry-After</c> is honored only on 429 and only in its
/// numeric-seconds form.
/// </remarks>
internal sealed class OutboundIQHttpClient : IDisposable
{
    private const int MaxBackoffMilliseconds = 8_000;
    private const string RequestIdHeader = "x-request-id";

    private readonly HttpClient _http;
    private readonly bool _ownsHttpClient;
    private readonly string _apiKey;
    private readonly string _baseUrl;
    private readonly string _userAgent;
    private readonly OutboundIQClientOptions _options;

    internal OutboundIQHttpClient(OutboundIQClientOptions options, HttpClient? httpClient)
    {
        ArgumentNullException.ThrowIfNull(options);

        _options = options.Clone();
        _options.Validate();

        var apiKey = _options.ApiKey;
        if (string.IsNullOrWhiteSpace(apiKey))
        {
            apiKey = Environment.GetEnvironmentVariable(OutboundIQClientOptions.ApiKeyEnvironmentVariable);
        }

        if (string.IsNullOrWhiteSpace(apiKey))
        {
            throw new OutboundIQException(
                "An API key is required. Pass it to the OutboundIQClient constructor, set "
                + $"OutboundIQClientOptions.ApiKey, or set the {OutboundIQClientOptions.ApiKeyEnvironmentVariable} "
                + "environment variable. Universal keys are generated in the outboundIQ workspace dashboard.");
        }

        _apiKey = apiKey!;
        _baseUrl = QueryString.NormalizeBaseUrl(_options.BaseUrl);
        _userAgent = BuildUserAgent(_options.UserAgentSuffix);

        if (httpClient is not null)
        {
            // Caller-owned. Never mutate it (HttpClient.Timeout throws once a request has been
            // sent) and never dispose it.
            _http = httpClient;
            _ownsHttpClient = false;
        }
        else
        {
            _http = CreateDefaultHttpClient();
            _ownsHttpClient = true;
        }
    }

    private static HttpClient CreateDefaultHttpClient()
    {
        var handler = new SocketsHttpHandler
        {
            // Lets a long-lived singleton client pick up DNS changes without IHttpClientFactory.
            PooledConnectionLifetime = TimeSpan.FromMinutes(2),
            PooledConnectionIdleTimeout = TimeSpan.FromMinutes(2),
            AutomaticDecompression = DecompressionMethods.All,
        };

        return new HttpClient(handler, disposeHandler: true)
        {
            // Timeouts are enforced per attempt by a linked CancellationTokenSource, so the
            // built-in whole-call timeout must not also apply.
            Timeout = System.Threading.Timeout.InfiniteTimeSpan,
        };
    }

    private static string BuildUserAgent(string? suffix)
    {
        var agent = $"outboundiq-client/{OutboundIQVersion.Value}";
        return string.IsNullOrWhiteSpace(suffix) ? agent : $"{agent} {suffix!.Trim()}";
    }

    internal static byte[] Serialize<T>(T value, JsonTypeInfo<T> typeInfo) =>
        JsonSerializer.SerializeToUtf8Bytes(value, typeInfo);

    public async Task<TResponse> SendAsync<TResponse>(
        ApiRequest request,
        JsonTypeInfo<TResponse> responseTypeInfo,
        CancellationToken cancellationToken)
    {
        var url = QueryString.BuildUrl(_baseUrl, request.Path, request.Query);

        // Only GET is assumed safe to repeat. A POST, PUT, or DELETE that failed mid-flight may
        // already have been applied, so replaying it could duplicate a write.
        var retrySafe = request.Method == HttpMethod.Get;

        for (var attempt = 0; ; attempt++)
        {
            HttpResponseMessage response;

            try
            {
                response = await SendOnceAsync(url, request, cancellationToken).ConfigureAwait(false);
            }
            catch (Exception ex) when (IsTransportFailure(ex, cancellationToken))
            {
                if (retrySafe && attempt < _options.MaxRetries)
                {
                    await Task.Delay(Backoff(attempt), cancellationToken).ConfigureAwait(false);
                    continue;
                }

                throw ToConnectionException(ex, url, request);
            }

            using (response)
            {
                var status = (int)response.StatusCode;

                if (response.IsSuccessStatusCode)
                {
                    return await DeserializeAsync(response, responseTypeInfo, request, cancellationToken)
                        .ConfigureAwait(false);
                }

                var body = await ResponseBody.ReadAsync(response, cancellationToken).ConfigureAwait(false);
                var requestId = ReadRequestId(response);

                if (status == 429)
                {
                    var retryAfter = ParseRetryAfter(response);

                    if (attempt < _options.MaxRetries)
                    {
                        await Task.Delay(retryAfter ?? Backoff(attempt), cancellationToken).ConfigureAwait(false);
                        continue;
                    }

                    throw new OutboundIQRateLimitException(
                        Decorate($"{request.Method} {request.Path} was rate limited (429)", requestId),
                        status,
                        body.Raw,
                        body.Json,
                        requestId,
                        retryAfter);
                }

                if (status >= 500 && retrySafe && attempt < _options.MaxRetries)
                {
                    await Task.Delay(Backoff(attempt), cancellationToken).ConfigureAwait(false);
                    continue;
                }

                throw ToApiException(status, body, requestId, request);
            }
        }
    }

    private async Task<HttpResponseMessage> SendOnceAsync(
        string url,
        ApiRequest request,
        CancellationToken cancellationToken)
    {
        // A fresh message per attempt: HttpClient refuses to resend one it has already sent, and
        // its content stream would already be consumed.
        using var message = new HttpRequestMessage(request.Method, url);
        message.Headers.TryAddWithoutValidation("Authorization", $"Bearer {_apiKey}");
        message.Headers.TryAddWithoutValidation("Accept", "application/json");
        message.Headers.TryAddWithoutValidation("User-Agent", _userAgent);

        if (request.Body is not null)
        {
            // ByteArrayContent with an explicit media type, rather than StringContent, so the
            // header is exactly "application/json" with no "; charset=utf-8" suffix.
            message.Content = new ByteArrayContent(request.Body);
            message.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
        }

        using var timeoutSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        timeoutSource.CancelAfter(_options.Timeout);

        return await _http
            .SendAsync(message, HttpCompletionOption.ResponseHeadersRead, timeoutSource.Token)
            .ConfigureAwait(false);
    }

    private static async Task<TResponse> DeserializeAsync<TResponse>(
        HttpResponseMessage response,
        JsonTypeInfo<TResponse> typeInfo,
        ApiRequest request,
        CancellationToken cancellationToken)
    {
#if NET8_0_OR_GREATER
        var text = await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);
#else
        var text = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
#endif
        var status = (int)response.StatusCode;
        var requestId = response.Headers.TryGetValues(RequestIdHeader, out var ids)
            ? ids.FirstOrDefault()
            : null;

        if (string.IsNullOrWhiteSpace(text))
        {
            throw new OutboundIQApiException(
                Decorate(
                    $"{request.Method} {request.Path} returned {status} with an empty body, but a JSON response was expected",
                    requestId),
                status,
                text,
                body: null,
                requestId);
        }

        TResponse? result;
        try
        {
            result = JsonSerializer.Deserialize(text, typeInfo);
        }
        catch (JsonException ex)
        {
            throw new OutboundIQApiException(
                Decorate($"{request.Method} {request.Path} returned {status} with a body that could not be parsed as JSON", requestId),
                status,
                text,
                body: null,
                requestId,
                ex);
        }

        if (result is null)
        {
            throw new OutboundIQApiException(
                Decorate($"{request.Method} {request.Path} returned {status} with a null JSON body", requestId),
                status,
                text,
                ResponseBody.Parse(text).Json,
                requestId);
        }

        return result;
    }

    private static OutboundIQApiException ToApiException(
        int status,
        ResponseBody body,
        string? requestId,
        ApiRequest request)
    {
        var detail = string.IsNullOrWhiteSpace(body.Message) ? string.Empty : $": {body.Message}";
        var summary = $"{request.Method} {request.Path} failed with status {status}{detail}";

        if (status is 401 or 403)
        {
            return new OutboundIQAuthenticationException(
                Decorate($"{summary}. Check that the API key is valid, active, and of type universal.", requestId),
                status,
                body.Raw,
                body.Json,
                requestId);
        }

        return new OutboundIQApiException(Decorate(summary, requestId), status, body.Raw, body.Json, requestId);
    }

    private OutboundIQConnectionException ToConnectionException(Exception ex, string url, ApiRequest request)
    {
        if (ex is OperationCanceledException or TimeoutException)
        {
            var milliseconds = _options.Timeout.TotalMilliseconds.ToString("0", CultureInfo.InvariantCulture);
            return new OutboundIQConnectionException(
                $"{request.Method} {request.Path} timed out after {milliseconds}ms", ex);
        }

        return new OutboundIQConnectionException($"Network error while calling {url}", ex);
    }

    /// <summary>
    /// Whether the failure is one a retry could plausibly fix, as opposed to the caller cancelling.
    /// </summary>
    private static bool IsTransportFailure(Exception ex, CancellationToken cancellationToken) =>
        ex is HttpRequestException
        || ex is TimeoutException
        // Our per-attempt timeout also surfaces as OperationCanceledException. The caller's own
        // cancellation must propagate untouched, so it is excluded here.
        || (ex is OperationCanceledException && !cancellationToken.IsCancellationRequested);

    private string? ReadRequestId(HttpResponseMessage response)
    {
        if (!_options.CaptureRequestId)
        {
            return null;
        }

        return response.Headers.TryGetValues(RequestIdHeader, out var values) ? values.FirstOrDefault() : null;
    }

    private static string Decorate(string message, string? requestId) =>
        string.IsNullOrWhiteSpace(requestId) ? message : $"{message} (request id: {requestId})";

    /// <summary>
    /// Reads <c>Retry-After</c> as a number of seconds.
    /// </summary>
    /// <remarks>
    /// The raw header is read rather than <see cref="HttpResponseHeaders.RetryAfter"/> because the
    /// typed accessor also parses the HTTP-date form, which the TypeScript SDK ignores. Matching
    /// that keeps backoff behavior identical across the two SDKs.
    /// </remarks>
    private static TimeSpan? ParseRetryAfter(HttpResponseMessage response)
    {
        if (!response.Headers.TryGetValues("Retry-After", out var values))
        {
            return null;
        }

        var raw = values.FirstOrDefault();
        if (string.IsNullOrWhiteSpace(raw))
        {
            return null;
        }

        // InvariantCulture matters: a machine with a comma decimal separator would otherwise
        // misread a value like "1.5".
        return double.TryParse(raw, NumberStyles.Float, CultureInfo.InvariantCulture, out var seconds)
            && double.IsFinite(seconds)
            && seconds >= 0
                ? TimeSpan.FromSeconds(seconds)
                : null;
    }

    private TimeSpan Backoff(int attempt)
    {
        var baseMilliseconds = Math.Min(
            _options.RetryBaseDelay.TotalMilliseconds * Math.Pow(2, attempt),
            MaxBackoffMilliseconds);

        // Random.Shared is thread-safe and lock-free; a per-call `new Random()` would correlate
        // across concurrent retries and defeat the jitter.
        var jitter = Random.Shared.NextDouble() * (_options.RetryBaseDelay.TotalMilliseconds / 2);

        return TimeSpan.FromMilliseconds(baseMilliseconds + jitter);
    }

    public void Dispose()
    {
        if (_ownsHttpClient)
        {
            _http.Dispose();
        }
    }
}
