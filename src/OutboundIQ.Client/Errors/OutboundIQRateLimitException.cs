using System.Text.Json;

namespace OutboundIQ;

/// <summary>
/// Raised on a 429 response after the configured retries were exhausted.
/// </summary>
public sealed class OutboundIQRateLimitException : OutboundIQApiException
{
    /// <summary>Initializes a new instance of the <see cref="OutboundIQRateLimitException"/> class.</summary>
    public OutboundIQRateLimitException()
    {
    }

    /// <summary>Initializes a new instance of the <see cref="OutboundIQRateLimitException"/> class.</summary>
    /// <param name="message">The error message.</param>
    public OutboundIQRateLimitException(string message)
        : base(message)
    {
    }

    /// <summary>Initializes a new instance of the <see cref="OutboundIQRateLimitException"/> class.</summary>
    /// <param name="message">The error message.</param>
    /// <param name="innerException">The underlying cause, if any.</param>
    public OutboundIQRateLimitException(string message, Exception? innerException)
        : base(message, innerException)
    {
    }

    /// <summary>Initializes a new instance of the <see cref="OutboundIQRateLimitException"/> class.</summary>
    /// <param name="message">The error message.</param>
    /// <param name="status">The HTTP status code of the response.</param>
    /// <param name="rawBody">The response body as received.</param>
    /// <param name="body">The response body parsed as JSON, when it was valid JSON.</param>
    /// <param name="requestId">The value of the <c>x-request-id</c> response header.</param>
    /// <param name="retryAfter">The value of the <c>Retry-After</c> response header.</param>
    public OutboundIQRateLimitException(
        string message,
        int? status,
        string? rawBody,
        JsonElement? body,
        string? requestId,
        TimeSpan? retryAfter)
        : base(message, status, rawBody, body, requestId)
    {
        RetryAfter = retryAfter;
    }

    /// <summary>
    /// How long the API asked the caller to wait, from the <c>Retry-After</c> response header.
    /// <see langword="null"/> when the header was absent or was not a number of seconds.
    /// </summary>
    public TimeSpan? RetryAfter { get; }
}
